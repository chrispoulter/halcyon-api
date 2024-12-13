using Halcyon.Api.Data.Users;

namespace Halcyon.Api.Services.Authentication;

public interface IJwtTokenGenerator
{
    public string GenerateJwtToken(User user);
}
