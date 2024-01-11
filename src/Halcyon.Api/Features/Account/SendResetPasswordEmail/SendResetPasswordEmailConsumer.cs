using Halcyon.Api.Services.Email;
using MassTransit;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer(IEmailSender emailSender)
    : IConsumer<SendResetPasswordEmailEvent>
{
    public async Task Consume(ConsumeContext<SendResetPasswordEmailEvent> context)
    {
        var message = context.Message;

        await emailSender.SendEmailAsync(new()
        {
            To = message.To,
            Template = "ResetPasswordEmail.html",
            Data = new { message.PasswordResetToken, message.SiteUrl }
        },
        context.CancellationToken);
    }
}
