namespace Halcyon.Api.Common.Email;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
