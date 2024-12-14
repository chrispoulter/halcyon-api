namespace Halcyon.Api.Services.Infrastructure;

public static class MailKitExtensions
{
    public static IHostApplicationBuilder AddMailKitClient(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        MailKit.Telemetry.SmtpClient.Configure();

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing =>
                tracing.AddSource(MailKit.Telemetry.SmtpClient.ActivitySourceName)
            )
            .WithMetrics(metrics => metrics.AddMeter(MailKit.Telemetry.SmtpClient.MeterName));

        return builder;
    }
}
