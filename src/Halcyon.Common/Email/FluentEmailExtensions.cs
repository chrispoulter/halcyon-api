using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Email;

public static class FluentEmailExtensions
{
    public static IHostApplicationBuilder AddFluentEmail(this IHostApplicationBuilder builder)
    {
        var emailSettings = new EmailSettings();
        builder.Configuration.Bind(EmailSettings.SectionName, emailSettings);

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
