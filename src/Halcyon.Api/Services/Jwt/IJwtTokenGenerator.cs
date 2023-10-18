using Halcyon.Api.Data;

namespace Halcyon.Api.Services.Jwt
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(User user);
    }
}
