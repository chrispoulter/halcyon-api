using System.Net;
using System.Net.Mail;

namespace Halcyon.Api.Common.Email;

public static class FluentEmailExtensions
{
    public static IHostApplicationBuilder AddFluentEmail(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        var emailSettings =
            builder.Configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>()
            ?? throw new InvalidOperationException(
                "Email settings section is missing in configuration."
            );

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
        {
            emailSettings.ParseConnectionString(connectionString);
        }

        builder
            .Services.AddFluentEmail(emailSettings.NoReplyAddress)
            .AddLiquidRenderer(configure =>
            {
                configure.ConfigureTemplateContext = (context, _) =>
                {
                    context.SetValue("SiteUrl", emailSettings.SiteUrl);
                };
            })
            .AddSmtpSender(
                new SmtpClient
                {
                    Host = emailSettings.SmtpServer,
                    Port = emailSettings.SmtpPort,
                    EnableSsl = emailSettings.SmtpSsl,
                    Credentials = new NetworkCredential(
                        emailSettings.SmtpUserName,
                        emailSettings.SmtpPassword
                    ),
                }
            );

        return builder;
    }
}
