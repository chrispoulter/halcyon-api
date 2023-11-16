﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Halcyon.Api.Services.Email;

public class EmailSender(
    ITemplateEngine templateEngine,
    IOptions<EmailSettings> emailSettings,
    ILogger<EmailSender> logger)
    : IEmailSender
{
    private readonly EmailSettings emailSettings = emailSettings.Value;

    public async Task SendEmailAsync(EmailMessage message)
    {
        logger.LogInformation("Sending email to {To} with template {Template}", message.To, message.Template);

        var (body, subject) = await templateEngine.RenderTemplateAsync(message.Template, message.Data);

        var email = new MimeMessage(
            new[] { MailboxAddress.Parse(emailSettings.NoReplyAddress) },
            new[] { MailboxAddress.Parse(message.To) },
            subject,
            new TextPart(TextFormat.Html) { Text = body });

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(emailSettings.SmtpServer, emailSettings.SmtpPort);
            await client.AuthenticateAsync(emailSettings.SmtpUserName, emailSettings.SmtpPassword);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
        catch (Exception error)
        {
            logger.LogError(error, "Email send failed");
        }
    }
}
