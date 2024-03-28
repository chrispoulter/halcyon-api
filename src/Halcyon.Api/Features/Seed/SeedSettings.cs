namespace Halcyon.Api.Features.Seed;

public record SeedSettings(List<SeedUser> Users)
{
    public static string SectionName { get; } = "Seed";
}
