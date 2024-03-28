using FluentValidation;

namespace Halcyon.Api.Features.Users.SearchUsers;

public enum UserSort
{
    EMAIL_ADDRESS_ASC,
    EMAIL_ADDRESS_DESC,
    NAME_ASC,
    NAME_DESC
}

public record SearchUsersRequest(
    string Search,
    UserSort Sort = UserSort.NAME_ASC,
    int Page = 1,
    int Size = 50
);

public class SearchUsersRequestValidator : AbstractValidator<SearchUsersRequest>
{
    public SearchUsersRequestValidator()
    {
        RuleFor(x => x.Sort).IsInEnum();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(int.MaxValue);
        RuleFor(x => x.Size).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
