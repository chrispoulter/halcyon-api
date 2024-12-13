using System.Reflection;
using MassTransit;

namespace Halcyon.Api.Services.Infrastructure;

public static class MassTransitExtensions
{
    public static IHostApplicationBuilder AddMassTransit(
        this IHostApplicationBuilder builder,
        string connectionName,
        Assembly assembly
    )
    {
        builder.Services.AddMassTransit(options =>
        {
            options.AddConsumers(assembly);

            options.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(builder.Configuration.GetConnectionString(connectionName));
                    cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(true));
                    //cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
                }
            );
        });

        return builder;
    }
}
