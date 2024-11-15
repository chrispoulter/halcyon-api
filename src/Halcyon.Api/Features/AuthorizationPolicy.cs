using Halcyon.Api.Data;

namespace Halcyon.Api.Features;

public class AuthorizationPolicy
{
    public static readonly string[] IsUserAdministrator =
    [
        Role.SystemAdministrator,
        Role.UserAdministrator
    ];
}
