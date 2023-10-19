using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Halcyon.Api.Services.Email;

public partial class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var resource = ReadResource($"{message.Template}.html");
        var title = GetTitle(message.Template);

        var subject = ReplaceData(title, message.Data);
        var body = ReplaceData(resource, message.Data);

        var email = new MimeMessage(
            new[] { MailboxAddress.Parse(_emailSettings.NoReplyAddress) },
            new[] { MailboxAddress.Parse(message.To) },
            subject,
            new TextPart(TextFormat.Html)
            {
                Text = body
            });

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

    private string ReadResource(string resource)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var name = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(resource));

        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private string GetTitle(string template)
    {
        var match = TitleRegex().Match(template);

        return match.Success
            ? match.Groups[1].Value
            : string.Empty;
    }

    private string ReplaceData(string template, IDictionary<string, string> data)
    {
        foreach (var entry in data)
        {
            template = template.Replace($"{{{{ {entry.Key} }}}}", entry.Value);
        }

        return template;
    }

    [GeneratedRegex("<title>\\s*(.+?)\\s*</title>")]
    private static partial Regex TitleRegex();
}
