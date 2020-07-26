using Halcyon.Web.Data;

namespace Halcyon.Web.Services.Jwt
{
    public interface IJwtService
    {
        public JwtResult GenerateToken(User user);
    }
}
