using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Halcyon.Api.Common.Infrastructure;

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
            {
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString(connectionName))
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(provider.GetServices<IInterceptor>());
            }
        );

        builder.EnrichNpgsqlDbContext<HalcyonDbContext>();

        return builder;
    }
}
