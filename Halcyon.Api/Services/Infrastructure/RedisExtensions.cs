namespace Halcyon.Api.Services.Infrastructure;

public static class RedisExtensions
{
    public static IHostApplicationBuilder AddRedis(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString(connectionName);
        });

#pragma warning disable EXTEXP0018
        builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

        return builder;
    }
}
