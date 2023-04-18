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
    [AuthorizeRole(Role.SYSTEM_ADMINISTRATOR, Role.USER_ADMINISTRATOR)]
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
        [ProducesResponseType(typeof(ApiResponse<SearchUsersResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query
                    .Where(u => (u.EmailAddress + " " + u.FirstName + " " + u.LastName)
                    .ToLower()
                    .Contains(request.Search.ToLower()));
            }

            var count = await query.CountAsync();

            query = request.Sort switch
            {
                UserSort.EMAIL_ADDRESS_DESC => query.OrderByDescending(r => r.EmailAddress),
                UserSort.EMAIL_ADDRESS_ASC => query.OrderBy(r => r.EmailAddress),
                UserSort.NAME_DESC => query.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName),
                _ => query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName),
            };

            if (request.Page > 1)
            {
                query = query.Skip((request.Page - 1) * request.Size);
            }

            query = query.Take(request.Size);

            var users = await query
                .Select(user => new GetUserResponse
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.ToUniversalTime(),
                    IsLockedOut = user.IsLockedOut,
                    Roles = user.Roles
                })
                .ToListAsync();

            var pageCount = (count + request.Size - 1) / request.Size;

            return Ok(new ApiResponse<SearchUsersResponse>
            {
                Data =
                {
                    Items = users,
                    HasNextPage = request.Page < pageCount,
                    HasPreviousPage = request.Page > 1
                }
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUser(int id)
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
                    Roles = user.Roles
                }
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing != null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.DUPLICATE_USER,
                    Message = $"User name \"{request.EmailAddress}\" is already taken."
                });
            }

            var user = new User
            {
                EmailAddress = request.EmailAddress,
                Password = _hashService.GenerateHash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth.Value.ToUniversalTime(),
                Roles = request.Roles
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.USER_CREATED,
                Message = "User successfully created.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
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

            if (!request.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

                if (existing != null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Code = InternalStatusCode.DUPLICATE_USER,
                        Message = $"User name \"{request.EmailAddress}\" is already taken."
                    });
                }
            }

            user.EmailAddress = request.EmailAddress;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DateOfBirth = request.DateOfBirth.Value.ToUniversalTime();
            user.Roles = request.Roles;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.USER_UPDATED,
                Message = "User successfully updated.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("{id}/lock")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
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

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.USER_LOCKED,
                Message = "User successfully locked.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("{id}/unlock")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
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

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.USER_UNLOCKED,
                Message = "User successfully unlocked.",
                Data = { Id = user.Id }
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
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

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.USER_DELETED,
                Message = "User successfully deleted.",
                Data = { Id = user.Id }
            });
        }
    }
}
