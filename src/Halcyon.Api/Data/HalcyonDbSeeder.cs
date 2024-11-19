using Halcyon.Api.Core.Authentication;
using Halcyon.Api.Core.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Data;

public class HalcyonDbSeeder(
    HalcyonDbContext dbContext,
    IPasswordHasher passwordHasher,
    IOptions<SeedSettings> seedSettings
) : IDbSeeder<HalcyonDbContext>
{
    private readonly SeedSettings seedSettings = seedSettings.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var emailAddresses = seedSettings.Users.Select(u => u.EmailAddress);

        var users = await dbContext
            .Users.Where(u => emailAddresses.Contains(u.EmailAddress))
            .ToListAsync(cancellationToken);

        foreach (var seedUser in seedSettings.Users)
        {
            var user = users.FirstOrDefault(u => u.EmailAddress == seedUser.EmailAddress);

            if (user is null)
            {
                user = new();
                dbContext.Users.Add(user);
            }

            seedUser.Adapt(user);
            user.Password = passwordHasher.HashPassword(seedUser.Password);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
