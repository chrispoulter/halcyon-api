namespace Halcyon.Api.Features.Manage.GetProfile;

public record GetProfileResponse(
    int Id,
    string EmailAddress,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    uint Version
);
