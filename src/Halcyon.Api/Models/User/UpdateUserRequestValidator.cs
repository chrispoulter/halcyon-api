using FluentValidation;
using Halcyon.Api.Extensions;
using Halcyon.Api.Services.Date;

namespace Halcyon.Api.Models.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator(IDateService dateService)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.DateOfBirth).NotEmpty().InThePast(dateService).WithName("Date Of Birth");
        }
    }
}