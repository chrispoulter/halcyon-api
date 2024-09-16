namespace Halcyon.Api.Features.Profile.GetProfile;

public class GetProfileResponse
{
    public Guid Id { get; set; }

    public string EmailAddress { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public uint Version { get; set; }
}
