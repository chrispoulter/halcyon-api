namespace Halcyon.Api.Core.Web;

public class CorsPolicySettings
{
    public static string SectionName { get; } = "CorsPolicy";

    public string[] AllowedOrigins { get; set; }

    public string[] AllowedMethods { get; set; }

    public string[] AllowedHeaders { get; set; }
}
