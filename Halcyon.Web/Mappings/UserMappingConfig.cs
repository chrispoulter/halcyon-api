using Halcyon.Web.Data;
using Halcyon.Web.Models.Account;
using Halcyon.Web.Models.User;
using Halcyon.Web.Settings;
using Mapster;

namespace Halcyon.Web.Mappings
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
