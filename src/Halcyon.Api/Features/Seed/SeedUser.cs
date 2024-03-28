namespace Halcyon.Api.Features.Seed;

public record SeedUser(
    string EmailAddress,
    string Password,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    List<string> Roles
);
