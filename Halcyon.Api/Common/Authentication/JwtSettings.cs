namespace Halcyon.Api.Common.Authentication;

public class JwtSettings
{
    public static string SectionName { get; } = "Jwt";

    public string SecurityKey { get; set; } = null!;

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public int ExpiresIn { get; set; }
}
