using Halcyon.Api.Data;

namespace Halcyon.Api.Core.Authentication;

public interface IJwtTokenGenerator
{
    public string GenerateJwtToken(User user);
}
