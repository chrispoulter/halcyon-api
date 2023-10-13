using Halcyon.Api.Data;

namespace Halcyon.Api.Services.Jwt
{
    public interface IJwtService
    {
        public Token CreateToken(User user);
    }
}
