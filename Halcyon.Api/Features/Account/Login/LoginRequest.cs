using FluentValidation;

namespace Halcyon.Api.Features.Account.Login;

public class LoginRequest
{
    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255)
            .WithName("Email Address");

        RuleFor(x => x.Password).NotEmpty().WithName("Password");
    }
}
