using Halcyon.Web.Data;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

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
