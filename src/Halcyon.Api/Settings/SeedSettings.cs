using Halcyon.Api.Models.Seed;

namespace Halcyon.Api.Settings
{
    public class SeedSettings
    {
        public static string SectionName { get; } = "Seed";

        public List<SeedUser> Users { get; set; }
    }
}
