using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Common.Database.Migration;

public interface IDbSeeder<in TDbContext>
    where TDbContext : DbContext
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
