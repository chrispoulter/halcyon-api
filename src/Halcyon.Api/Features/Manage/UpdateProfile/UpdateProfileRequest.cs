using FluentValidation;
using Halcyon.Api.Common;
using Halcyon.Api.Services.Validators;

namespace Halcyon.Api.Features.Manage.UpdateProfile;

public record UpdateProfileRequest(
    uint? Version,
    string EmailAddress,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth
) : UpdateRequest(Version);

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255)
            .WithName("Email Address");

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
        RuleFor(x => x.DateOfBirth).NotEmpty().InThePast(timeProvider).WithName("Date Of Birth");
    }
}
