using FluentValidation;
using Halcyon.Web.Services.Date;

namespace Halcyon.Web.Models.Manage
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IDateService dateService)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.DateOfBirth).NotEmpty().LessThan(dateService.UtcNow).WithName("Date Of Birth");
        }   
    }
}