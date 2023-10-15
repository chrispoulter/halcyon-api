using Halcyon.Api.Data;
using Mapster;

namespace Halcyon.Api.Features.Users.CreateUser
{
    public class CreateUserRequestMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(CreateUserRequest, string), User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}