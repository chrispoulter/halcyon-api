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

        [HttpPost]
        public async Task<IActionResult> SeedData()
        {
            await _context.Database.MigrateAsync();

            var systemRole = await _context.Roles
                .FirstOrDefaultAsync(u => u.Name == Roles.SystemAdministrator);

            if (systemRole == null)
            {
                systemRole = new Role
                {
                    Name = Roles.SystemAdministrator
                };

                _context.Roles.Add(systemRole);

                await _context.SaveChangesAsync();
            }

            var userRole = await _context.Roles
                .FirstOrDefaultAsync(u => u.Name == Roles.UserAdministrator);

            if (userRole == null)
            {
                userRole = new Role
                {
                    Name = Roles.UserAdministrator
                };

                _context.Roles.Add(userRole);

                await _context.SaveChangesAsync();
            }

            var systemUser = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == _seedSettings.EmailAddress);

            if (systemUser != null)
            {
                _context.Users.Remove(systemUser);

                await _context.SaveChangesAsync();
            }

            systemUser = new User
            {
                EmailAddress = _seedSettings.EmailAddress,
                Password = _hashService.GenerateHash(_seedSettings.Password),
                FirstName = "System",
                LastName = "Administrator",
                DateOfBirth = new DateTime(1970, 1, 1),
                UserRoles = new [] { new UserRole { Role = systemRole } }
            };

            _context.Users.Add(systemUser);

            await _context.SaveChangesAsync();

            var result = new UserCreatedResult
            {
                UserId = systemUser.Id
            };

            return Ok("User successfully created.", result);
        }
    }
}
