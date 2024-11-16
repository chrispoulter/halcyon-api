using Halcyon.Api.Data;

namespace Halcyon.Api.Features;

public class AuthPolicy
{
    public static readonly string[] IsUserAdministrator =
    [
        Role.SystemAdministrator,
        Role.UserAdministrator
    ];
}
