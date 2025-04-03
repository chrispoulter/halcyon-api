using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Email;

public static class EmailExtensions
{
    public static IHostApplicationBuilder AddFluentEmail(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        var emailSettings = new EmailSettings();
        builder.Configuration.Bind(EmailSettings.SectionName, emailSettings);

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
                    context.SetValue("CdnUrl", emailSettings.CdnUrl);
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
