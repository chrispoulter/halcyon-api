using FluentValidation;

namespace Halcyon.Api.Models.Token
{
    public class CreateTokenRequestValidator : AbstractValidator<CreateTokenRequest>
    {
        public CreateTokenRequestValidator()
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
            RuleFor(x => x.Password).NotEmpty().WithName("Password");
        }   
    }   
}