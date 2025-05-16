namespace Halcyon.Api.Common.Authentication;

public static class SecurityExtensions
{
    public static IHostApplicationBuilder AddSecurityServices(this IHostApplicationBuilder builder)
    {
        var jwtConfig = builder.Configuration.GetSection(JwtSettings.SectionName);
        builder.Services.Configure<JwtSettings>(jwtConfig);
        builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return builder;
    }
}
