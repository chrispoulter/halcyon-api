using FluentValidation;

namespace Halcyon.Api.Features.Token;

public class TokenRequest
{
    public string EmailAddress { get; set; }

    public string Password { get; set; }
}

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
        RuleFor(x => x.Password).NotEmpty().WithName("Password");
    }
}