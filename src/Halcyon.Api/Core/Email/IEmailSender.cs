namespace Halcyon.Api.Core.Email;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
