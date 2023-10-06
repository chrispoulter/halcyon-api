using Halcyon.Web.Data;
using Halcyon.Web.Filters;
using Halcyon.Web.Models;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AuthorizeRole(Role.SYSTEM_ADMINISTRATOR, Role.USER_ADMINISTRATOR)]
    [Produces("application/json")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
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
        [ProducesResponseType(typeof(SearchUsersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(u => EF.Functions.ILike(u.Search, $"%{request.Search}%"));
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
                .Select(user => new SearchUserResponse
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsLockedOut = user.IsLockedOut,
                    Roles = user.Roles
                })
                .ToListAsync();

            var pageCount = (count + request.Size - 1) / request.Size;

            return Ok(new SearchUsersResponse()
            {
                Items = users,
                HasNextPage = request.Page < pageCount,
                HasPreviousPage = request.Page > 1
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            return Ok(new GetUserResponse()
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth.ToUniversalTime(),
                IsLockedOut = user.IsLockedOut,
                Roles = user.Roles,
                Version = user.Version
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing is not null)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "User name is already taken."
                );
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

            return Ok(new UpdateResponse { Id = user.Id });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request.Version is not null && request.Version != user.Version)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            if (!request.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

                if (existing is not null)
                {
                    return Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "User name is already taken."
                    );
                }
            }

            user.EmailAddress = request.EmailAddress;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DateOfBirth = request.DateOfBirth.Value.ToUniversalTime();
            user.Roles = request.Roles;

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }

        [HttpPut("{id}/lock")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> LockUser(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            if (user.Id == CurrentUserId)
            {
                return Problem(
                     statusCode: StatusCodes.Status400BadRequest,
                     title: "Cannot lock currently logged in user."
                 );
            }

            user.IsLockedOut = true;

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }

        [HttpPut("{id}/unlock")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UnlockUser(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            user.IsLockedOut = false;

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteUser(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
        {
            var user = await _context.Users
               .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            if (user.Id == CurrentUserId)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot delete currently logged in user."
                );
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
