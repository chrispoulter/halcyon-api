using Halcyon.Api.Data;
using Halcyon.Api.Services.Date;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Halcyon.Api.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly JwtSettings _jwtSettings;

        public JwtService(
            IDateTimeProvider dateTimeProvider, 
            IOptions<JwtSettings> jwtSettings)
        {
            _dateTimeProvider = dateTimeProvider;
            _jwtSettings = jwtSettings.Value;
        }

        public JwtToken CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(JwtClaimNames.Roles, role.ToString()));
            }

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims: claims,
                expires: _dateTimeProvider.UtcNow.AddSeconds(_jwtSettings.ExpiresIn),
                signingCredentials: credentials);

            return new JwtToken
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = _jwtSettings.ExpiresIn,
                TokenType = "Bearer"
            };
        }
    }
}
