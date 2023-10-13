using Halcyon.Api.Data;

namespace Halcyon.Api.Settings
{
    public class SeedSettings
    {
        public static string SectionName { get; } = "Seed";

        public List<SeedUser> Users { get; set; }
    }

    public class SeedUser
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public List<Role> Roles { get; set; }
    }
}
