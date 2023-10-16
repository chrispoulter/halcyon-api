using FluentValidation;
using Halcyon.Api.Validators;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Models.Account
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IOptions<CorsOptions> corsOptions)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithName("Email Address");
            RuleFor(x => x.SiteUrl).NotEmpty().ReturnUrl(corsOptions).WithName("Site Url");
        }
    }
}