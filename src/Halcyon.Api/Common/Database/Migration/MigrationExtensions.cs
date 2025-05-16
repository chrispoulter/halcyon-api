using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Common.Database.Migration;

public static class MigrationExtensions
{
    public static IServiceCollection AddMigration<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddHostedService<MigrationBackgroundService<TDbContext>>();

        return services;
    }

    public static IServiceCollection AddMigration<TDbContext, TDbSeeder>(
        this IServiceCollection services
    )
        where TDbContext : DbContext
        where TDbSeeder : class, IDbSeeder<TDbContext>
    {
        services.AddHostedService<MigrationBackgroundService<TDbContext>>();
        services.AddScoped<IDbSeeder<TDbContext>, TDbSeeder>();

        return services;
    }
}
