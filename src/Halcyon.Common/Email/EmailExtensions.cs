using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Email;

public static class EmailExtensions
{
    public static IHostApplicationBuilder AddEmailServices(this IHostApplicationBuilder builder)
    {
        var emailSettings = new EmailSettings();
        builder.Configuration.Bind(EmailSettings.SectionName, emailSettings);

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
                    Host = "localhost",
                    Port = 1025,
                    EnableSsl = false,
                    Credentials = new NetworkCredential("mail-dev", "password"),
                }
            );

        return builder;
    }
}
