namespace Halcyon.Web.Models.Token
{
    public class CreateTokenRequest
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }
    }
}