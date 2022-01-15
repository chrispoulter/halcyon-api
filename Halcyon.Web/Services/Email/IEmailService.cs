namespace Halcyon.Web.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
