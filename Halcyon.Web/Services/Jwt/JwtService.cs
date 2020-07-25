using Halcyon.Web.Data;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Jwt
{
    public class JwtService : IJwtService
    {
        public Task<bool> VerifyToken(string token)
        {
            return Task.FromResult(true);
        }

        public Task<JwtResult> GenerateToken(User user)
        {
            return Task.FromResult(new JwtResult 
            { 
                AccessToken = "12345",
                ExpiresIn = 3600
            });
        }
    }
}
