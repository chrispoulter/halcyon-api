namespace Halcyon.Api.Features;

public static class Role
{
    public const string SystemAdministrator = "SYSTEM_ADMINISTRATOR";

    public const string UserAdministrator = "USER_ADMINISTRATOR";
}

public class AuthPolicy
{
    public static readonly string[] IsUserAdministrator =
    [
        Role.SystemAdministrator,
        Role.UserAdministrator,
    ];
}
