using System.Security.Claims;
using System.Text;
using Halcyon.Api.Data.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Halcyon.Api.Services.Authentication;

public class JwtTokenGenerator(TimeProvider timeProvider, IOptions<JwtSettings> jwtSettings)
    : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey)
        );

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Email, user.EmailAddress),
                    new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                    new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    .. (user.Roles ?? []).Select(role => new Claim(
                        JwtClaimNames.Roles,
                        role.ToString()
                    )),
                ]
            ),
            Expires = timeProvider.GetUtcNow().AddSeconds(_jwtSettings.ExpiresIn).UtcDateTime,
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }
}
