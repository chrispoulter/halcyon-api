using FluentValidation;
using Halcyon.Web.Services.Date;

namespace Halcyon.Web.Models.Account
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
      public RegisterRequestValidator(IDateService dateService)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(50);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.DateOfBirth).NotEmpty().LessThan(dateService.UtcNow).WithName("Date Of Birth");
        }
    }
}