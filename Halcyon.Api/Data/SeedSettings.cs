namespace Halcyon.Api.Data;

public class SeedSettings
{
    public static string SectionName { get; } = "Seed";

    public required List<SeedUser> Users { get; set; }

    public class SeedUser
    {
        public required string EmailAddress { get; set; }

        public required string Password { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public required List<string> Roles { get; set; }
    }
}
