namespace Halcyon.Web.Services.Jwt
{
    public class Token
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public string TokenType { get; set; }
    }
}