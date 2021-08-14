using Halcyon.Web.Models.Events;
using Halcyon.Web.Services.Email;
using Halcyon.Web.Services.Events;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Halcyon.Web.BackgroundServices
{
    public class SendEmailBackgroundService : BackgroundService
    {
        private readonly IEventService _eventService;

        private readonly IEmailService _emailService;

        public SendEmailBackgroundService(
            IEventService eventService,
            IEmailService emailService)
        {
            _eventService = eventService;
            _emailService = emailService;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            async Task handler(SendEmailEvent data)
            {
                var message = new EmailMessage
                {
                    Template = data.Template,
                    Data = data.Context
                };

                message.To.Add(data.EmailAddress);

                await _emailService.SendEmailAsync(message);
            }

            return _eventService.HandleEventAsync<SendEmailEvent>(handler, cancellationToken);
        }
    }
}
