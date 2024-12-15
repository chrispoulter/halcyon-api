using static MailKit.Telemetry;

namespace Halcyon.Api.Services.Infrastructure;

public static class MailKitExtensions
{
    public static IHostApplicationBuilder AddMailKitClient(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        //SmtpClient.Configure();

        //builder
        //    .Services.AddOpenTelemetry()
        //    .WithTracing(tracing => tracing.AddSource(SmtpClient.ActivitySourceName))
        //    .WithMetrics(metrics => metrics.AddMeter(SmtpClient.MeterName));

        return builder;
    }
}
