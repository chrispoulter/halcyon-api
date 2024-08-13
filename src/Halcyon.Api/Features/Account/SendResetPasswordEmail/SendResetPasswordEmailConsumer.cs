using Halcyon.Api.Core.Email;
using MassTransit;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer(IEmailSender emailSender)
    : IConsumer<Batch<SendResetPasswordEmailEvent>>
{
    private const string _template = "ResetPasswordEmail.html";

    public async Task Consume(ConsumeContext<Batch<SendResetPasswordEmailEvent>> context)
    {
        var batch = context.Message.DistinctBy(d => new
        {
            d.Message.To,
            d.Message.PasswordResetToken,
            d.Message.SiteUrl
        });

        foreach (var item in batch)
        {
            var message = item.Message;

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
}
