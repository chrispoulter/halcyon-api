﻿namespace Halcyon.Api.Services.Infrastructure;

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
        });

        return builder;
    }
}
