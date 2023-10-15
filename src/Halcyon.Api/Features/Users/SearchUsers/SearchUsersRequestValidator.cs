using FluentValidation;

namespace Halcyon.Api.Features.Users.SearchUsers
{
    public class SearchUsersRequestValidator : AbstractValidator<SearchUsersRequest>
    {
        public SearchUsersRequestValidator()
        {
            RuleFor(x => x.Sort).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(int.MaxValue);
            RuleFor(x => x.Size).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        }
    }
}
