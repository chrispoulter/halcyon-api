using Halcyon.Web.Data;
using Halcyon.Web.Filters;
using Halcyon.Web.Models;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Forbidden)]
    [Route("[controller]")]
    [AuthorizeRole(Roles.SYSTEM_ADMINISTRATOR, Roles.USER_ADMINISTRATOR)]
    public class UserController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        public UserController(HalcyonDbContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ListUsersResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ListUsers([FromQuery] ListUsersModel model)
        {
            var page = Math.Max(model.Page ?? 1, 1);
            var size = Math.Min(model.Size ?? 50, 50);

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(u => (u.EmailAddress + " " + u.FirstName + " " + u.LastName).ToLower().Contains(model.Search.ToLower()));
            }

            var count = await query.CountAsync();

            query = model.Sort switch
            {
                UserSort.EMAIL_ADDRESS_DESC => query.OrderByDescending(r => r.EmailAddress),
                UserSort.EMAIL_ADDRESS_ASC => query.OrderBy(r => r.EmailAddress),
                UserSort.NAME_DESC => query.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName),
                _ => query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName),
            };

            if (page > 1)
            {
                query = query.Skip((page - 1) * size);
            }

            query = query.Take(size);

            var users = await query
                .Select(user => new GetUserResponse
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.ToUniversalTime(),
                    IsLockedOut = user.IsLockedOut,
                    Roles = user.UserRoles
                        .Select(ur => ur.Role.Name)
                        .ToList()
                })
                .ToListAsync();

            var pageCount = (count + size - 1) / size;

            return Ok(new ApiResponse<ListUsersResponse>
            {
                Data =
                {
                    Items = users,
                    HasNextPage = page < pageCount,
                    HasPreviousPage = page > 1
                }
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponse<GetUserResponse>
            {
                Data =
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.ToUniversalTime(),
                    IsLockedOut = user.IsLockedOut,
                    Roles = user.UserRoles
                        .Select(ur => ur.Role.Name)
                        .ToList()
                }
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (existing != null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.DUPLICATE_USER,
                    Message = $"User name \"{model.EmailAddress}\" is already taken."
                });
            }

            var user = new User
            {
                EmailAddress = model.EmailAddress,
                Password = _hashService.GenerateHash(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.Value.ToUniversalTime()
            };

            user.UserRoles.Clear();

            var roles = await _context.Roles
                .Where(r => model.Roles.Contains(r.Name))
                .ToListAsync();

            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole { RoleId = role.Id });
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.USER_CREATED,
                Message = "User successfully created.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            if (!model.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

                if (existing != null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Code = InternalStatusCode.DUPLICATE_USER,
                        Message = $"User name \"{model.EmailAddress}\" is already taken."
                    });
                }
            }

            user.EmailAddress = model.EmailAddress;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth.Value.ToUniversalTime();

            user.UserRoles.Clear();

            var roles = await _context.Roles
                .Where(r => model.Roles.Contains(r.Name))
                .ToListAsync();

            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole { RoleId = role.Id });
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.USER_UPDATED,
                Message = "User successfully updated.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("{id}/lock")]
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LockUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            if (user.Id == CurrentUserId)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.LOCK_CURRENT_USER,
                    Message = "Cannot lock currently logged in user."
                });
            }

            user.IsLockedOut = true;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.USER_LOCKED,
                Message = "User successfully locked.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("{id}/unlock")]
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            user.IsLockedOut = false;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.USER_UNLOCKED,
                Message = "User successfully unlocked.",
                Data = { Id = user.Id }
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
               .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            if (user.Id == CurrentUserId)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.DELETE_CURRENT_USER,
                    Message = "Cannot delete currently logged in user."
                });
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.USER_DELETED,
                Message = "User successfully deleted.",
                Data = { Id = user.Id }
            });
        }
    }
}
