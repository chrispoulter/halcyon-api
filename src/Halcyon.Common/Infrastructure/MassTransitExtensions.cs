using System.Reflection;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Infrastructure;

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

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(DiagnosticHeaders.DefaultListenerName))
            .WithMetrics(metrics => metrics.AddMeter(InstrumentationOptions.MeterName));

        return builder;
    }
}
