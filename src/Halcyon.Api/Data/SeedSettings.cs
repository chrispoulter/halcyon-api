﻿namespace Halcyon.Api.Data;

public class SeedSettings
{
    public static string SectionName { get; } = "Seed";

    public List<SeedUser> Users { get; set; }

    public class SeedUser
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public List<string> Roles { get; set; }
    }
}
