using FluentValidation;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordRequest
{
    public string EmailAddress { get; set; }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithName("Email Address");
    }
}
