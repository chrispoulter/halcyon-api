using Halcyon.Web.Data;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.UserAdministrator)]
    public class UserController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordService _hashService;

        public UserController(HalcyonDbContext context, IPasswordService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        [HttpGet]
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

            switch (model.Sort)
            {
                case UserSort.EmailAddressDesc:
                    query = query.OrderByDescending(r => r.EmailAddress);
                    break;

                case UserSort.EmailAddressAsc:
                    query = query.OrderBy(r => r.EmailAddress);
                    break;

                case UserSort.NameDesc:
                    query = query.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName);
                    break;

                default:
                    query = query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName);
                    break;
            }

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
                    Roles = user.UserRoles.Select(ur => ur.RoleId).ToList()
                })
                .ToListAsync();

            var result = new ListUsersResponse
            {
                Items = users,
                Page = page,
                Size = size,
                Total = count
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = new GetUserResponse
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth.ToUniversalTime(),
                IsLockedOut = user.IsLockedOut,
                Roles = user.UserRoles.Select(ur => ur.RoleId).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (existing != null)
            {
                return BadRequest($"User name \"{model.EmailAddress}\" is already taken.");
            }

            var user = new User
            {
                EmailAddress = model.EmailAddress,
                Password = _hashService.GenerateHash(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.ToUniversalTime()
            };

            user.UserRoles.Clear();

            foreach (var roleId in model.Roles)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var result = new UserCreatedResponse
            {
                UserId = user.Id
            };

            return Ok(result, "User successfully created.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!model.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

                if (existing != null)
                {
                    return BadRequest($"User name \"{model.EmailAddress}\" is already taken.");
                }
            }

            user.EmailAddress = model.EmailAddress;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth.ToUniversalTime();

            user.UserRoles.Clear();

            foreach (var roleId in model.Roles)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }

            await _context.SaveChangesAsync();

            return Ok("User successfully updated.");
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockUser(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Id == CurrentUserId)
            {
                return BadRequest("Cannot lock currently logged in user.");
            }

            user.IsLockedOut = true;

            await _context.SaveChangesAsync();

            return Ok("User successfully locked.");
        }

        [HttpPut("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsLockedOut = false;

            await _context.SaveChangesAsync();

            return Ok("User successfully unlocked.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
               .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Id == CurrentUserId)
            {
                return BadRequest("Cannot delete currently logged in user.");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("User successfully deleted.");
        }
    }
}
