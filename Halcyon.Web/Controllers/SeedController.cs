using Halcyon.Web.Data;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Password;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeedController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordService _hashService;

        private readonly SeedSettings _seedSettings;

        public SeedController(
            HalcyonDbContext context, 
            IPasswordService hashService,
            IOptions<SeedSettings> seedSettings)
        {
            _context = context;
            _hashService = hashService;
            _seedSettings = seedSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> SeedData()
        {
            await _context.Database.MigrateAsync();

            var systemRole = await AddRoleAsync(Roles.SystemAdministrator);
            var userRole = await AddRoleAsync(Roles.UserAdministrator);
            var systemUser = await AddSystemUserAsync(new[] { systemRole, userRole });

            var result = new UserCreatedResult
            {
                UserId = systemUser.Id
            };

            return Ok("User successfully created.", result);
        }

        private async Task<Role> AddRoleAsync(string name)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(u => u.Name == name);

            if (role == null)
            {
                role = new Role
                {
                    Name = name
                };

                _context.Roles.Add(role);

                await _context.SaveChangesAsync();
            }

            return role;
        }

        private async Task<User> AddSystemUserAsync(Role[] roles)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.EmailAddress == _seedSettings.EmailAddress);

            if (user == null)
            {
                user = new User();
                _context.Users.Add(user);
            }

            user.EmailAddress = _seedSettings.EmailAddress;
            user.Password = _hashService.GenerateHash(_seedSettings.Password);
            user.FirstName = "System";
            user.LastName = "Administrator";
            user.DateOfBirth = new DateTime(1970, 1, 1);

            user.UserRoles.Clear();
            
            foreach (var role in roles)
            { 
                user.UserRoles.Add(new UserRole { RoleId = role.Id });
            }

            await _context.SaveChangesAsync();

            return user;
        }
    }
}
