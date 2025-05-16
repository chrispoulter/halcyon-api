namespace Halcyon.Api.Common.Authentication;

public interface IJwtTokenGenerator
{
    public string GenerateJwtToken(IJwtUser user);
}
