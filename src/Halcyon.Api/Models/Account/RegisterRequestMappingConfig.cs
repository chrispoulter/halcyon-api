using Mapster;

namespace Halcyon.Api.Models.Account
{
    public class RegisterRequestMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<(RegisterRequest, string), Data.User>()
                .Map(dest => dest, src => src.Item1)
                .Map(dest => dest.Password, src => src.Item2);
        }
    }
}
