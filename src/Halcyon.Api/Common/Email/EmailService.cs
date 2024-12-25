using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Halcyon.Api.Common.Email;

public class EmailService(
    ITemplateEngine templateEngine,
    IOptions<EmailSettings> emailSettings,
    ILogger<EmailService> logger
) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendEmailAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation(
            "Sending email to {To} with template {Template}",
            message.To,
            message.Template
        );

        var model = new { message.Data, _emailSettings.CdnUrl };

        var (body, subject) = await templateEngine.RenderTemplateAsync(
            message.Template,
            model,
            cancellationToken
        );

        var email = new MimeMessage(
            [MailboxAddress.Parse(_emailSettings.NoReplyAddress)],
            [MailboxAddress.Parse(message.To)],
            subject,
            new TextPart(TextFormat.Html) { Text = body }
        );

        try
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.SmtpPort,
                _emailSettings.SmtpSsl,
                cancellationToken: cancellationToken
            );

            if (
                !string.IsNullOrEmpty(_emailSettings.SmtpUserName)
                && !string.IsNullOrEmpty(_emailSettings.SmtpPassword)
            )
            {
                await client.AuthenticateAsync(
                    _emailSettings.SmtpUserName,
                    _emailSettings.SmtpPassword,
                    cancellationToken
                );
            }

            await client.SendAsync(email, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while sending email to {To} with template {Template}",
                message.To,
                message.Template
            );
        }
    }
}
