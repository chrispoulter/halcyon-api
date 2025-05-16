using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Halcyon.Api.Common.Database;

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
            {
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString(connectionName))
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(provider.GetServices<IInterceptor>());
            }
        );

        builder.EnrichNpgsqlDbContext<TContext>();

        return builder;
    }
}
