namespace Halcyon.Web.Services.Jwt
{
    public class TokenResult
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public string TokenType { get; set; }
    }
}