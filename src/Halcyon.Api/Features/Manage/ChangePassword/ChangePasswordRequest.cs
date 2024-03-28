using FluentValidation;
using Halcyon.Api.Common;

namespace Halcyon.Api.Features.Manage.ChangePassword;

public record ChangePasswordRequest(uint? Version, string CurrentPassword, string NewPassword)
    : UpdateRequest(Version);

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithName("Current Password");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(50)
            .WithName("New Password");
    }
}
