using Halcyon.Web.Data;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Jwt
{
    public interface IJwtService
    {
        public Task<bool> VerifyToken(string token);

        public Task<JwtResult> GenerateToken(User user);
    }
}
