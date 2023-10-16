using Mapster;

namespace Halcyon.Api.Models.Seed
{

    public class SeedUserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(SeedUser, string), Data.User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}
