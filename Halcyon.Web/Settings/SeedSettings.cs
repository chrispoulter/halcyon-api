using Halcyon.Web.Data;
using Mapster;

namespace Halcyon.Web.Settings
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

    public class SeedUserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(SeedUser, string), User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}
