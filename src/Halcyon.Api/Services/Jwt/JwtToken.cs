namespace Halcyon.Api.Services.Jwt
{
    public class JwtToken
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public string TokenType { get; set; }
    }
}