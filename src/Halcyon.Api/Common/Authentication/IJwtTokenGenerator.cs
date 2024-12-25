using Halcyon.Api.Data.Users;

namespace Halcyon.Api.Common.Authentication;

public interface IJwtTokenGenerator
{
    public string GenerateJwtToken(User user);
}
