namespace Halcyon.Api.Features.Profile.GetProfile;

public class GetProfileResponse
{
    public Guid Id { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public uint Version { get; set; }
}
