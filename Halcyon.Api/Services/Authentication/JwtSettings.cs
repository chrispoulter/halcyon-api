namespace Halcyon.Api.Services.Authentication;

public class JwtSettings
{
    public static string SectionName { get; } = "Jwt";

    public string SecurityKey { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public int ExpiresIn { get; set; }
}
