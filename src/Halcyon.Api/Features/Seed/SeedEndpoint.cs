using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Features.Seed;

public class SeedEndpoint : IEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/seed", HandleAsync)
            .WithTags("Seed")
            .Produces<string>(contentType: "text/plain");

        return endpoints;
    }

    public static async Task<IResult> HandleAsync(
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<SeedSettings> seedSettings)
    {
        await dbContext.Database.MigrateAsync();

        if (seedSettings.Value.Users != null)
        {
            var emailAddresses = seedSettings.Value.Users
                .Select(u => u.EmailAddress);

            var users = await dbContext.Users
                .Where(u => emailAddresses.Contains(u.EmailAddress))
            .ToListAsync();

            foreach (var seedUser in seedSettings.Value.Users)
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
        }

        await dbContext.SaveChangesAsync();

        return Results.Content("Environment seeded.");
    }
}
