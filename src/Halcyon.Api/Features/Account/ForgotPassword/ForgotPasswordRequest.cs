namespace Halcyon.Api.Features.Account.ForgotPassword
{
    public class ForgotPasswordRequest
    {
        public string EmailAddress { get; set; }

        public string SiteUrl { get; set; }
    }
}