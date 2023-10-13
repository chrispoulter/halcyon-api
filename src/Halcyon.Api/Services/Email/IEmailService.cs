namespace Halcyon.Api.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
