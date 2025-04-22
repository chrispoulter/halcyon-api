using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Halcyon.Common.Database.Migration;

public static class MigrationExtensions
{
    public static IServiceCollection AddMigration<TDbContext, TDbSeeder>(
        this IServiceCollection services
    )
        where TDbContext : DbContext
        where TDbSeeder : class, IDbSeeder<TDbContext>
    {
        services.AddHostedService<MigrationHostedService<TDbContext>>();
        services.AddScoped<IDbSeeder<TDbContext>, TDbSeeder>();

        return services;
    }
}
