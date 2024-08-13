namespace Halcyon.Api.Data;

public class User : IEntityWithId
{
    public Guid Id { get; set; }

    public string EmailAddress { get; set; }

    public string Password { get; set; }

    public Guid? PasswordResetToken { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public bool IsLockedOut { get; set; }

    public List<string> Roles { get; set; }

    public uint Version { get; }

    public string Search { get; }
}
