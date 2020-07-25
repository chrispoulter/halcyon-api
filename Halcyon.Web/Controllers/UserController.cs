using Halcyon.Web.Data;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = SystemRoles.UserAdministrator)]
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
        public IActionResult ListUsers()
        {
            var result = new ListUsersResult();
            return Ok(result);
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
                Password = await _hashService.GenerateHashAsync(model.Password),
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
