using FluentValidation;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Halcyon.Web.Models.Account
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IOptions<CorsOptions> corsOptions)
        {
            RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithName("Email Address");
            
            RuleFor(x => x.SiteUrl).NotEmpty()
                .WithName("Site Url")
                .Custom((url, context) => {
                    if (string.IsNullOrEmpty(url))
                    {
                        return;
                    }

                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    {
                        context.AddFailure($"The {context.DisplayName} field is not an allowed origin.");
                    }

                    var origin = uri.GetLeftPart(UriPartial.Authority);

                    var corsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);

                    if (corsPolicy is null || corsPolicy.AllowAnyOrigin)
                    {
                        return;
                    }

                    var isAllowed = corsPolicy
                        .Origins
                        .Any(pattern => Regex.IsMatch(origin, Regex.Escape(pattern).Replace("\\*", "(.+)"), RegexOptions.IgnoreCase));

                    if (!isAllowed)
                    {
                        context.AddFailure($"The {context.DisplayName} field is not an allowed origin.");
                    }
                });
        }
    }
}