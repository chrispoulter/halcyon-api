namespace Halcyon.Api.Services.Authorization;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireRole(
        this RouteHandlerBuilder builder,
        params string[] roles
    )
    {
        return builder.RequireAuthorization(policy => policy.RequireRole(roles));
    }
}
