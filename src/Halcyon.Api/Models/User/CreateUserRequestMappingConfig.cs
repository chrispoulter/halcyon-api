using Mapster;

namespace Halcyon.Api.Models.User
{
    public class CreateUserRequestMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(CreateUserRequest, string), Data.User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}
