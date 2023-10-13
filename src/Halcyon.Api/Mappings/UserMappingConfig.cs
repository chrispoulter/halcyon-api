using Halcyon.Api.Data;
using Halcyon.Api.Models.Account;
using Halcyon.Api.Models.User;
using Halcyon.Api.Settings;
using Mapster;

namespace Halcyon.Api.Mappings
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(RegisterRequest, string), User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);

            config
                .NewConfig<(CreateUserRequest, string), User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);

            config
                .NewConfig<(SeedUser, string), User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}
