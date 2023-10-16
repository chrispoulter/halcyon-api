﻿using FluentValidation;

namespace Halcyon.Api.Models.Token
{
    public class CreateTokenRequest
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }
    }

    public class CreateTokenRequestValidator : AbstractValidator<CreateTokenRequest>
    {
        public CreateTokenRequestValidator()
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(255).WithName("Email Address");
            RuleFor(x => x.Password).NotEmpty().WithName("Password");
        }
    }
}