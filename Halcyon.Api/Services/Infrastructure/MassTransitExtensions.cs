using System.Reflection;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;

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

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(DiagnosticHeaders.DefaultListenerName))
            .WithMetrics(metrics => metrics.AddMeter(InstrumentationOptions.MeterName));

        return builder;
    }
}
