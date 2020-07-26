using Halcyon.Web.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Halcyon.Web.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public JwtResult GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiresIn),
                signingCredentials: credentials);


            return new JwtResult 
            { 
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = _jwtSettings.ExpiresIn
            };
        }
    }
}
