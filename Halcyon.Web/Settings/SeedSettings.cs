namespace Halcyon.Web.Settings
{
    public class SeedSettings
    {
        public static string SectionName { get; } = "Seed";

        public string EmailAddress { get; set; }

        public string Password { get; set; }
    }
}
