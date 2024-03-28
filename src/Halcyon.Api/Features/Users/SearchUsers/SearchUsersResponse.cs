namespace Halcyon.Api.Features.Users.SearchUsers;

public record SearchUsersResponse(
    List<SearchUserResponse> Items,
    bool HasNextPage,
    bool HasPreviousPage
);

public record SearchUserResponse(
    int Id,
    string EmailAddress,
    string FirstName,
    string LastName,
    bool IsLockedOut,
    List<string> Roles
);
