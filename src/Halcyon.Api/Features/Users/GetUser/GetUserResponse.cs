namespace Halcyon.Api.Features.Users.GetUser;

public record GetUserResponse(
    int Id,
    string EmailAddress,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    bool IsLockedOut,
    List<string> Roles,
    uint Version
);
