using Halcyon.Api.Services.Email;
using MassTransit;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer : IConsumer<SendResetPasswordEmailEvent>
{
    private readonly IEmailSender _emailSender;

    private readonly ITemplateEngine _templateEngine;

    public SendResetPasswordEmailConsumer(IEmailSender emailSender, ITemplateEngine templateEngine)
    {
        _emailSender = emailSender;
        _templateEngine = templateEngine;
    }

    public async Task Consume(ConsumeContext<SendResetPasswordEmailEvent> context)
    {
        var message = context.Message;

        var (body, subject) = await _templateEngine.RenderTemplateAsync(
            "ResetPasswordEmail.html",
            new { message.PasswordResetToken, message.SiteUrl });

        await _emailSender.SendEmailAsync(new()
        {
            To = message.To,
            Subject = subject,
            Body = body,
        });
    }
}
