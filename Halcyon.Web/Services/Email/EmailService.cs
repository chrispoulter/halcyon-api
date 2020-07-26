using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Email
{
    public class EmailService : IEmailService
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

            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                IsBodyHtml = true,
                Body = body,
            };

            foreach (var emailAddress in message.To)
            {
                mailMessage.To.Add(new MailAddress(emailAddress));
            }

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

        public string ReadResource(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var name = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(resource));

            using var stream = assembly.GetManifestResourceStream(name);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public string GetTitle(string template)
        {
            var match = Regex.Match(template, @"<title>\s*(.+?)\s*</title>");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public string ReplaceData(string template, Dictionary<string, string> data)
        {
            foreach (var entry in data)
            {
                template = template.Replace($"{{{{ {entry.Key} }}}}", entry.Value);
            }

            return template;
        }
    }
}
