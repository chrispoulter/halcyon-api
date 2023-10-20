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

        await _emailSender.SendEmailAsync(new()
        {
            To = message.To,
            Template = "ResetPasswordEmail.html",
            Data = new { message.PasswordResetToken, message.SiteUrl }
        });
    }
}
