using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Services.Hash;
using Halcyon.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class SeedController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        private readonly SeedSettings _seedSettings;

        public SeedController(
            HalcyonDbContext context,
            IHashService hashService,
            IOptions<SeedSettings> seedSettings)
        {
            _context = context;
            _hashService = hashService;
            _seedSettings = seedSettings.Value;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Index()
        {
            await _context.Database.MigrateAsync();

            var roleIds = new List<int>();

            foreach (Roles role in Enum.GetValues(typeof(Roles)))
            {
                var roleId = await AddRoleAsync(role.ToString());
                roleIds.Add(roleId);
            }

            await AddSystemUserAsync(roleIds);

            return Ok(new ApiResponse
            {
                Code = InternalStatusCode.ENVIRONMENT_SEEDED,
                Message = "Environment seeded."
            });
        }

        private async Task<int> AddRoleAsync(string name)
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

            return role.Id;
        }

        private async Task<int> AddSystemUserAsync(List<int> roleIds)
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
            user.DateOfBirth = new DateTime(1970, 1, 1).ToUniversalTime();

            user.UserRoles.Clear();

            foreach (var roleId in roleIds)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }

            await _context.SaveChangesAsync();

            return user.Id;
        }
    }
}
