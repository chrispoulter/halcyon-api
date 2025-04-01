using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Infrastructure;

public static class RedisExtensions
{
    public static IHostApplicationBuilder AddRedisDistributedCache(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString(connectionName);
            options.InstanceName = builder.Environment.ApplicationName;
        });

        return builder;
    }
}
