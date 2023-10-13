namespace Halcyon.Api.Models.Account
{
    public class ForgotPasswordRequest
    {
        public string EmailAddress { get; set; }

        public string SiteUrl { get; set; }
    }
}