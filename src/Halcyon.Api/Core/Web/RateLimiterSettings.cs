namespace Halcyon.Api.Core.Web;

public class RateLimiterSettings
{
    public static string SectionName { get; } = "RateLimiter";

    public int PermitLimit { get; set; }

    public int Window { get; set; }

    public int QueueLimit { get; set; }
}
