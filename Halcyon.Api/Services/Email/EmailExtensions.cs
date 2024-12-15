using static MailKit.Telemetry;

namespace Halcyon.Api.Services.Email;

public static class EmailExtensions
{
    public static IHostApplicationBuilder AddEmailServices(this IHostApplicationBuilder builder)
    {
        var emailConfig = builder.Configuration.GetSection(EmailSettings.SectionName);
        builder.Services.Configure<EmailSettings>(emailConfig);
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<ITemplateEngine, TemplateEngine>();

        SmtpClient.Configure();

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(SmtpClient.ActivitySourceName))
            .WithMetrics(metrics => metrics.AddMeter(SmtpClient.MeterName));

        return builder;
    }
}
