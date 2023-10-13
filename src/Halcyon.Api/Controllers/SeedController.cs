using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Settings;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("text/plain")]
    public class SeedController : ControllerBase
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Index()
        {
            await _context.Database.MigrateAsync();

            if (_seedSettings.Users != null)
            {
                var emailAddresses = _seedSettings.Users
                    .Select(u => u.EmailAddress);

                var users = await _context.Users
                    .Where(u => emailAddresses.Contains(u.EmailAddress))
                    .ToListAsync();

                foreach (var seedUser in _seedSettings.Users)
                {
                    var user = users.FirstOrDefault(u => u.EmailAddress == seedUser.EmailAddress);

                    if (user is null)
                    {
                        user = new();

                        _context.Users.Add(user);
                    }

                    var password = _hashService.GenerateHash(seedUser.Password);
                    (seedUser, password).Adapt(user);
                }
            }

            await _context.SaveChangesAsync();

            return Content("Environment seeded.");
        }
    }
}
