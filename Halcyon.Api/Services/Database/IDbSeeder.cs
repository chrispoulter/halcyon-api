using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Services.Database;

public interface IDbSeeder<in TDbContext>
    where TDbContext : DbContext
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
