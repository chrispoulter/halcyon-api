using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Common.Database;

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

        services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
                tracing.AddSource(MigrationHostedService<TDbContext>.ActivitySourceName)
            );

        return services;
    }

    public static void SetExceptionTags(this Activity activity, Exception ex)
    {
        activity.AddTag("exception.message", ex.Message);
        activity.AddTag("exception.stacktrace", ex.ToString());
        activity.AddTag("exception.type", ex.GetType().FullName);
        activity.SetStatus(ActivityStatusCode.Error);
    }
}
