using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Features.Seed;

public class SeedController : BaseController
{
    private readonly HalcyonDbContext _context;

    private readonly IPasswordHasher _passwordHasher;

    private readonly SeedSettings _seedSettings;

    public SeedController(
        HalcyonDbContext context,
        IPasswordHasher passwordHasher,
        IOptions<SeedSettings> seedSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _seedSettings = seedSettings.Value;
    }

    [HttpGet("/seed")]
    [Tags("Seed")]
    [Produces("text/plain")]
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

                seedUser.Adapt(user);
                user.Password = _passwordHasher.GenerateHash(seedUser.Password);
            }
        }

        await _context.SaveChangesAsync();

        return Content("Environment seeded.");
    }
}
