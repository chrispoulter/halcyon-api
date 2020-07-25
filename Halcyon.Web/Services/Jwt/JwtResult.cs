namespace Halcyon.Web.Services.Jwt
{
    public class JwtResult
    {
        public string AccessToken { get; set; }

        public int ExpiresIn { get; set; }
    }
}
