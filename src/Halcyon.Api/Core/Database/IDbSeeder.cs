using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Core.Database;

public interface IDbSeeder<in TDbContext>
    where TDbContext : DbContext
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
