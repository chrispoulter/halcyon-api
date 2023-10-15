using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.Register;
using Halcyon.Api.Features.Seed;
using Halcyon.Api.Features.Users.CreateUser;
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
