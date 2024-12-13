using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Services.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddMigration<TDbContext, TDbSeeder>(
        this IServiceCollection services
    )
        where TDbContext : DbContext
        where TDbSeeder : class, IDbSeeder<TDbContext>
    {
        services.AddHostedService<MigrationHostedService<TDbContext>>();
        services.AddScoped<IDbSeeder<TDbContext>, TDbSeeder>();

        //services
        //    .AddOpenTelemetry()
        //    .WithTracing(tracing =>
        //        tracing.AddSource(MigrationHostedService<TDbContext>.ActivitySourceName)
        //    );

        return services;
    }
}
