using FluentValidation;
using Halcyon.Api.Services.Date;
using Halcyon.Api.Services.Validators;

namespace Halcyon.Api.Features.Users.UpdateUser;

public class UpdateUserRequest : UpdateRequest
{
    public string EmailAddress { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public List<string> Roles { get; set; }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
        RuleFor(x => x.DateOfBirth).NotEmpty().InThePast(dateTimeProvider).WithName("Date Of Birth");
    }
}