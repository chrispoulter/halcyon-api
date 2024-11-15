namespace Halcyon.Api.Core.Web;

public class RateLimiterSettings
{
    public static string SectionName { get; } = "RateLimiter";

    public int ReplenishmentPeriod { get; set; }

    public int QueueLimit { get; set; }

    public int TokenLimit { get; set; }

    public int TokenLimit2 { get; set; }

    public int TokensPerPeriod { get; set; }

    public bool AutoReplenishment { get; set; }
}
