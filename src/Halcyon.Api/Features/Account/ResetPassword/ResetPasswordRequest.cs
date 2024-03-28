using FluentValidation;

namespace Halcyon.Api.Features.Account.ResetPassword;

public class ResetPasswordRequest
{
    public Guid Token { get; set; }

    public string EmailAddress { get; set; }

    public string NewPassword { get; set; }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithName("Email Address");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(50)
            .WithName("New Password");
    }
}
