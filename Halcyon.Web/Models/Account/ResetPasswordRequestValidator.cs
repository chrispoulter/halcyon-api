using FluentValidation;

namespace Halcyon.Web.Models.Account
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithName("Email Address");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(50).WithName("New Password");
        }
    }
}