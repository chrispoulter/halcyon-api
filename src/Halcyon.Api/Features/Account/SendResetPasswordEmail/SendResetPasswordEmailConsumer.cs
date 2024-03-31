using Halcyon.Api.Services.Email;
using MassTransit;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer(IEmailSender emailSender)
    : IConsumer<SendResetPasswordEmailEvent>
{
    private const string _template = "ResetPasswordEmail.html";

    public async Task Consume(ConsumeContext<SendResetPasswordEmailEvent> context)
    {
        var message = context.Message;

        await emailSender.SendEmailAsync(
            new()
            {
                Template = _template,
                To = message.To,
                Data = new { message.PasswordResetToken, message.SiteUrl }
            },
            context.CancellationToken
        );
    }
}
