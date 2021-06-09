using Halcyon.Web.Data;
using Halcyon.Web.Filters;
using Halcyon.Web.Models;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Forbidden)]
    [Route("api/[controller]")]
    [AuthorizeRole(Roles.SystemAdministrator, Roles.UserAdministrator)]
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
                query = query.Where(u => (u.EmailAddress + " " + u.FirstName + " " + u.LastName).Contains(model.Search));
            }

            var count = await query.CountAsync();

            query = model.Sort switch
            {
                UserSort.EmailAddressDesc => query.OrderByDescending(r => r.EmailAddress),
                UserSort.EmailAddressAsc => query.OrderBy(r => r.EmailAddress),
                UserSort.NameDesc => query.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName),
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

            var result = new ListUsersResponse
            {
                Items = users,
                HasNextPage = page < pageCount,
                HasPreviousPage = page > 1
            };

            return Generate(HttpStatusCode.OK, result);
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
                return Generate(
                    HttpStatusCode.NotFound,
                    InternalStatusCode.USER_NOT_FOUND,
                    "User not found.");
            }

            var result = new GetUserResponse
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
            };

            return Generate(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (existing != null)
            {
                return Generate(
                    HttpStatusCode.BadRequest,
                    InternalStatusCode.DUPLICATE_USER,
                    $"User name \"{model.EmailAddress}\" is already taken.");
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

            var result = new UserCreatedResponse
            {
                UserId = user.Id
            };

            return Generate(
                HttpStatusCode.OK,
                InternalStatusCode.USER_CREATED,
                result,
                "User successfully created.");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Generate(
                    HttpStatusCode.NotFound,
                    InternalStatusCode.USER_NOT_FOUND,
                    "User not found.");
            }

            if (!model.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

                if (existing != null)
                {
                    return Generate(
                        HttpStatusCode.BadRequest,
                        InternalStatusCode.DUPLICATE_USER,
                        $"User name \"{model.EmailAddress}\" is already taken.");
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

            return Generate(
                HttpStatusCode.OK,
                InternalStatusCode.USER_UPDATED,
                "User successfully updated.");
        }

        [HttpPut("{id}/lock")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LockUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Generate(
                    HttpStatusCode.NotFound,
                    InternalStatusCode.USER_NOT_FOUND,
                    "User not found.");
            }

            if (user.Id == CurrentUserId)
            {
                return Generate(
                    HttpStatusCode.BadRequest,
                    InternalStatusCode.LOCK_CURRENT_USER,
                    "Cannot lock currently logged in user.");
            }

            user.IsLockedOut = true;

            await _context.SaveChangesAsync();

            return Generate(
                HttpStatusCode.OK,
                InternalStatusCode.USER_LOCKED,
                "User successfully locked.");
        }

        [HttpPut("{id}/unlock")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Generate(
                    HttpStatusCode.NotFound,
                    InternalStatusCode.USER_NOT_FOUND,
                    "User not found.");
            }

            user.IsLockedOut = false;

            await _context.SaveChangesAsync();

            return Generate(
                HttpStatusCode.OK,
                InternalStatusCode.USER_UNLOCKED,
                "User successfully unlocked.");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
               .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Generate(
                    HttpStatusCode.NotFound,
                    InternalStatusCode.USER_NOT_FOUND,
                    "User not found.");
            }

            if (user.Id == CurrentUserId)
            {
                return Generate(
                    HttpStatusCode.BadRequest,
                    InternalStatusCode.DELETE_CURRENT_USER,
                    "Cannot delete currently logged in user.");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Generate(
                HttpStatusCode.OK,
                InternalStatusCode.USER_DELETED,
                "User successfully deleted.");
        }
    }
}
