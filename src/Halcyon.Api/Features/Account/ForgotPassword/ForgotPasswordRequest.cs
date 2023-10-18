using FluentValidation;
using Halcyon.Api.Services.Validators;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Features.Account.ForgotPassword
{
    public class ForgotPasswordRequest
    {
        public string EmailAddress { get; set; }

        public string SiteUrl { get; set; }
    }

    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IOptions<CorsOptions> corsOptions)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithName("Email Address");
            RuleFor(x => x.SiteUrl).NotEmpty().ReturnUrl(corsOptions).WithName("Site Url");
        }
    }
}