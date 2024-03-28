using FluentValidation;
using Halcyon.Api.Common;
using Halcyon.Api.Services.Validators;

namespace Halcyon.Api.Features.Users.UpdateUser;

public record UpdateUserRequest(
    uint? Version,
    string EmailAddress,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    List<string> Roles
) : UpdateRequest(Version);

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(TimeProvider timeProvider)
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
