using Halcyon.Api.Services.Email;
using MassTransit;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer : IConsumer<SendResetPasswordEmailEvent>
{
    private readonly IEmailSender _emailSender;

    public SendResetPasswordEmailConsumer(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Consume(ConsumeContext<SendResetPasswordEmailEvent> context)
    {
        var message = context.Message;

        var email = new EmailMessage
        {
            Template = "ResetPasswordEmail.html",
            To = message.To,
            Data = new { message.PasswordResetToken, message.SiteUrl }
        };

        await _emailSender.SendEmailAsync(email);
    }
}
