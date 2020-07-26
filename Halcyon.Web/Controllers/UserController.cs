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
    [Route("[controller]")]
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
        public async Task<IActionResult> ListUsers(ListUsersModel model)
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
                    query.OrderByDescending(r => r.EmailAddress);
                    break;

                case UserSort.EmailAddressAsc:
                    query.OrderBy(r => r.EmailAddress);
                    break;

                case UserSort.NameDesc:
                    query.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName);
                    break;

                default:
                    query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName);
                    break;
            }

            query = query.Skip(page - 1 * size).Take(size);

            var users = await query
                .Select(u => new UserResult
                {
                    Id = u.Id,
                    EmailAddress = u.EmailAddress,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })
                .ToListAsync();

            var result = new ListUsersResult
            {
                Items = users,
                Page = page,
                Size = size,
                Total = count
            };

            return Ok(null, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || user.IsLockedOut)
            {
                return NotFound("User not found.");
            }

            var result = new GetUserResult
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
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
                DateOfBirth = model.DateOfBirth,
                UserRoles = model.Roles.Select(id => new UserRole { RoleId = id }).ToList()
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var result = new UserCreatedResult
            {
                UserId = user.Id
            };

            return Ok("User successfully created.", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel model)
        {
            var user = await _context.Users
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

            user.EmailAddress = user.EmailAddress;
            user.FirstName = user.FirstName;
            user.LastName = user.LastName;
            user.DateOfBirth = user.DateOfBirth;
            user.UserRoles = model.Roles.Select(id => new UserRole { RoleId = id }).ToList();

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
