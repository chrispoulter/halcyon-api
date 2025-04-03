using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Halcyon.Common.Database;

public static class EntityFrameworkExtensions
{
    public static IHostApplicationBuilder AddDbContext<TContext>(
        this IHostApplicationBuilder builder,
        string connectionName
    )
        where TContext : DbContext
    {
        builder.Services.AddDbContext<TContext>(
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

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddNpgsql())
            .WithMetrics(metrics => metrics.AddNpgsqlInstrumentation());

        return builder;
    }
}
