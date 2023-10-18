namespace Halcyon.Api.Features.Seed
{
    public class SeedSettings
    {
        public static string SectionName { get; } = "Seed";

        public List<SeedUser> Users { get; set; }
    }
}
