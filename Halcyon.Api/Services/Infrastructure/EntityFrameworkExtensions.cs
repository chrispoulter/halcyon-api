using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Halcyon.Api.Services.Infrastructure;

public static class EntityFrameworkExtensions
{
    public static IHostApplicationBuilder AddDbContext<TContext>(
        this IHostApplicationBuilder builder,
        string connectionName
    )
        where TContext : DbContext
    {
        builder.Services.AddDbContext<HalcyonDbContext>(
            (provider, options) =>
                options
                    .UseNpgsql(
                        builder.Configuration.GetConnectionString(connectionName),
                        builder => builder.EnableRetryOnFailure()
                    )
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(provider.GetServices<IInterceptor>())
        );

        builder.Services.AddHealthChecks().AddDbContextCheck<TContext>();

        //builder
        //    .Services.AddOpenTelemetry()
        //    .WithTracing(tracing => tracing.AddNpgsql())
        //    .WithMetrics(metrics => metrics.AddNpgsqlInstrumentation());

        return builder;
    }
}
