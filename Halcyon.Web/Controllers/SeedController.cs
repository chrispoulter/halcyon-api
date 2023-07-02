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

            var user = await _context.Users
                     .FirstOrDefaultAsync(u => u.EmailAddress == _seedSettings.EmailAddress);

            if (user == null)
            {
                user = new User
                {
                    Version = Guid.NewGuid()
                };

                _context.Users.Add(user);
            }

            user.EmailAddress = _seedSettings.EmailAddress;
            user.Password = _hashService.GenerateHash(_seedSettings.Password);
            user.FirstName = "System";
            user.LastName = "Administrator";
            user.DateOfBirth = new DateTime(1970, 1, 1).ToUniversalTime();
            user.Roles = new List<Role> { Role.SYSTEM_ADMINISTRATOR };

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse
            {
                Code = "ENVIRONMENT_SEEDED",
                Message = "Environment seeded."
            });
        }
    }
}
