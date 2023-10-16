using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Models.User
{
    public enum UserSort
    {
        EMAIL_ADDRESS_ASC,
        EMAIL_ADDRESS_DESC,
        NAME_ASC,
        NAME_DESC
    }

    public class SearchUsersRequest
    {
        public string Search { get; set; }

        public UserSort Sort { get; set; } = UserSort.NAME_ASC;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int Size { get; set; } = 50;
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
}
