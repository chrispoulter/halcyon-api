using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Features.Seed;

public class SeedController : BaseController
{
    private readonly HalcyonDbContext _dbContext;

    private readonly IPasswordHasher _passwordHasher;

    private readonly SeedSettings _seedSettings;

    public SeedController(
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<SeedSettings> seedSettings)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _seedSettings = seedSettings.Value;
    }

    [HttpGet("/seed")]
    [Tags("Seed")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Index()
    {
        await _dbContext.Database.MigrateAsync();

        if (_seedSettings.Users != null)
        {
            var emailAddresses = _seedSettings.Users
                .Select(u => u.EmailAddress);

            var users = await _dbContext.Users
                .Where(u => emailAddresses.Contains(u.EmailAddress))
                .ToListAsync();

            foreach (var seedUser in _seedSettings.Users)
            {
                var user = users.FirstOrDefault(u => u.EmailAddress == seedUser.EmailAddress);

                if (user is null)
                {
                    user = new();

                    _dbContext.Users.Add(user);
                }

                seedUser.Adapt(user);
                user.Password = _passwordHasher.HashPassword(seedUser.Password);
            }
        }

        await _dbContext.SaveChangesAsync();

        return Content("Environment seeded.");
    }
}
