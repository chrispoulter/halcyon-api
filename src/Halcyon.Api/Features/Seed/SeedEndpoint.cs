using Halcyon.Api.Core.Authentication;
using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Features.Seed;

public class SeedEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/seed", HandleAsync)
            .WithTags(EndpointTag.Seed)
            .Produces<string>(contentType: "text/plain");
    }

    private static async Task<IResult> HandleAsync(
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<SeedSettings> seedSettings,
        CancellationToken cancellationToken = default
    )
    {
        if (seedSettings.Value.Users is not null)
        {
            var emailAddresses = seedSettings.Value.Users.Select(u => u.EmailAddress);

            var users = await dbContext
                .Users.Where(u => emailAddresses.Contains(u.EmailAddress))
                .ToListAsync(cancellationToken);

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

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Results.Content("Environment seeded.");
    }
}
