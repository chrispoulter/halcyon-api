namespace Halcyon.Api.Features.Users.GetUser;

public class GetUserResponse
{
    public Guid Id { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public bool IsLockedOut { get; set; }

    public List<string>? Roles { get; set; }

    public uint Version { get; set; }
}
