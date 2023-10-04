using FluentValidation;
using Halcyon.Web.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Halcyon.Web.Models.Account
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