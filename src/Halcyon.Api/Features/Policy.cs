using Halcyon.Api.Data;

namespace Halcyon.Api.Features;

public class Policy
{
    public static readonly string[] IsUserAdministrator =
    [
        Role.SystemAdministrator,
        Role.UserAdministrator,
    ];
}
