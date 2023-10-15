namespace Halcyon.Api.Features.Account.ResetPassword
{
    public class ResetPasswordRequest
    {
        public Guid Token { get; set; }

        public string EmailAddress { get; set; }

        public string NewPassword { get; set; }
    }
}