﻿using FluentValidation;

namespace Halcyon.Web.Models.Manage
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        { 
            RuleFor(x => x.CurrentPassword).NotEmpty().WithName("Current Password");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(50).WithName("New Password");
        }
    }
}