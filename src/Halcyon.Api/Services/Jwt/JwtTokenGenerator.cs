﻿using Halcyon.Api.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Halcyon.Api.Services.Jwt;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly TimeProvider timeProvider;

    private readonly JwtSettings jwtSettings;

    public JwtTokenGenerator(
        TimeProvider timeProvider, 
        IOptions<JwtSettings> jwtSettings)
    {
        this.timeProvider = timeProvider;
        this.jwtSettings = jwtSettings.Value;
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.EmailAddress),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new(JwtClaimNames.Roles, role.ToString()));
        }

        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims: claims,
            expires: timeProvider.GetUtcNow().AddSeconds(jwtSettings.ExpiresIn).UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
