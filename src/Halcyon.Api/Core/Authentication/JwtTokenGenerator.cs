using System.Security.Claims;
using System.Text;
using Halcyon.Api.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Halcyon.Api.Core.Authentication;

public class JwtTokenGenerator(TimeProvider timeProvider, IOptions<JwtSettings> jwtSettings)
    : IJwtTokenGenerator
{
    private readonly JwtSettings jwtSettings = jwtSettings.Value;

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));

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
            Expires = timeProvider.GetUtcNow().AddSeconds(jwtSettings.ExpiresIn).UtcDateTime,
            SigningCredentials = credentials,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }
}
