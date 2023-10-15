using Halcyon.Api.Data;
using Mapster;

namespace Halcyon.Api.Features.Account.Register
{
    public class RegisterMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(RegisterRequest, string), User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}