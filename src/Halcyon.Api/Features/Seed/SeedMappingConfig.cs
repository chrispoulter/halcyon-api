using Halcyon.Api.Data;
using Mapster;

namespace Halcyon.Api.Features.Seed
{
    public class SeedMappingConfig : IRegister
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
