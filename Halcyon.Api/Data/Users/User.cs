using Halcyon.Api.Common.Authentication;
using NpgsqlTypes;

namespace Halcyon.Api.Data.Users;

public class User : IJwtUser
{
    public Guid Id { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid? PasswordResetToken { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public bool IsLockedOut { get; set; }

    public List<string>? Roles { get; set; }

    public uint Version { get; }

    public NpgsqlTsVector SearchVector { get; } = null!;
}
