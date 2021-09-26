using Halcyon.Web.Models.Events;
using Halcyon.Web.Services.Email;
using Halcyon.Web.Services.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Halcyon.Web.BackgroundServices
{
    public class SendEmailBackgroundService : EventBackgroundService<SendEmailEvent>
    {
        private readonly IEmailService _emailService;

        public SendEmailBackgroundService(
            IEmailService emailService,
            IOptions<EventSettings> eventSettings,
            ILogger<SendEmailBackgroundService> logger) 
            : base(eventSettings, logger)
        {
            _emailService = emailService;
        }

        public override async Task ProcessMessage(SendEmailEvent data)
        {
            var message = new EmailMessage
            {
                Template = data.Template,
                Data = data.Context
            };

            message.To.Add(data.EmailAddress);

            await _emailService.SendEmailAsync(message);
        }
    }
}
