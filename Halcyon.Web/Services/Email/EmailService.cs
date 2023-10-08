using Halcyon.Web.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Halcyon.Web.Services.Email
{
    public partial class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var template = ReadResource($"{message.Template}.html");
            var title = GetTitle(template);

            var subject = ReplaceData(title, message.Data);
            var body = ReplaceData(template, message.Data);

            var from = string.IsNullOrEmpty(message.From)
                ? _emailSettings.NoReplyAddress
                : message.From;

            var mailMessage = new MailMessage(from, message.To)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body,
            };

            try
            {
                using var client = new SmtpClient
                {
                    Host = _emailSettings.SmtpServer,
                    Port = _emailSettings.SmtpPort,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.SmtpUserName, _emailSettings.SmtpPassword)
                };

                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException error)
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

        private string ReplaceData(string template, Dictionary<string, string> data)
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
}
