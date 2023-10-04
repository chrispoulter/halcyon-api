namespace Halcyon.Web.Models.Account
{
    public class ResetPasswordRequest
    {
        public Guid Token { get; set; }

        public string EmailAddress { get; set; }

        public string NewPassword { get; set; }
    }
}