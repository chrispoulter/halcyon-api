using FluentValidation;

namespace Halcyon.Api.Features.Users.SearchUsers;

public enum UserSort
{
    EMAIL_ADDRESS_ASC,
    EMAIL_ADDRESS_DESC,
    NAME_ASC,
    NAME_DESC,
}

public class SearchUsersRequest
{
    public string Search { get; set; }

    public UserSort? Sort { get; set; }

    public int? Page { get; set; }

    public int? Size { get; set; }
}

public class SearchUsersRequestValidator : AbstractValidator<SearchUsersRequest>
{
    public SearchUsersRequestValidator()
    {
        RuleFor(x => x.Sort).IsInEnum();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(int.MaxValue);
        RuleFor(x => x.Size).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}
