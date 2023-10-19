using Halcyon.Api.Services.Email;
using MassTransit;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer : IConsumer<SendResetPasswordEmailEvent>
{
    private readonly IEmailSender _emailService;

    public SendResetPasswordEmailConsumer(IEmailSender emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<SendResetPasswordEmailEvent> context)
    {
        var message = context.Message;

        var email = new EmailMessage
        {
            To = message.To,
            Template = "RESET_PASSWORD",
            Data = new()
            {
                { "ResetPasswordUrl", $"{message.SiteUrl}/reset-password/{message.PasswordResetToken}" },
                { "SiteUrl", message.SiteUrl }
            }
        };

        await _emailService.SendEmailAsync(email);
    }
}
