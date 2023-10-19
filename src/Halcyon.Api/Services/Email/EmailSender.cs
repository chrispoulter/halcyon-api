using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Halcyon.Api.Services.Email;

public class EmailSender : IEmailSender
{
    private readonly ITemplateEngine _templateEngine;

    private readonly EmailSettings _emailSettings;

    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        ITemplateEngine templateEngine,
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailSender> logger)
    {
        _templateEngine = templateEngine;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var (body, subject) = await _templateEngine.RenderTemplateAsync(message.Template, message.Data);

        var email = new MimeMessage(
            new[] { MailboxAddress.Parse(_emailSettings.NoReplyAddress) },
            new[] { MailboxAddress.Parse(message.To) },
            subject,
            new TextPart(TextFormat.Html) { Text = body });

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort);
            await client.AuthenticateAsync(_emailSettings.SmtpUserName, _emailSettings.SmtpPassword);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
        catch (Exception error)
        {
            _logger.LogError(error, "Email Send Failed");
        }
    }
}
