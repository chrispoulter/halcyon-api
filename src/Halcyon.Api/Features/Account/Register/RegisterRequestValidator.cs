﻿using FluentValidation;
using Halcyon.Api.Services.Date;

namespace Halcyon.Api.Features.Account.Register
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator(IDateService dateService)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(50);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.DateOfBirth).NotEmpty().InThePast(dateService).WithName("Date Of Birth");
        }
    }
}