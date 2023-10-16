using FluentValidation;
using Halcyon.Api.Services.Date;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Halcyon.Api.Extensions
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, DateOnly?> InThePast<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder, IDateService dateService)
            => ruleBuilder
                .LessThan(DateOnly.FromDateTime(dateService.UtcNow))
                .WithMessage("'{PropertyName}' must be in the past.");

        public static IRuleBuilderOptions<T, string> ReturnUrl<T>(this IRuleBuilder<T, string> ruleBuilder, IOptions<CorsOptions> corsOptions)
            => ruleBuilder
                .Must(url =>
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        return true;
                    }

                    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    {
                        return false;
                    }

                    var origin = uri.GetLeftPart(UriPartial.Authority);
                    var corsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);

                    if (corsPolicy is null || corsPolicy.AllowAnyOrigin)
                    {
                        return true;
                    }

                    var isAllowed = corsPolicy
                        .Origins
                        .Any(pattern => Regex.IsMatch(origin, Regex.Escape(pattern).Replace("\\*", "(.+)"), RegexOptions.IgnoreCase));

                    if (!isAllowed)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage("'{PropertyName}' is not an allowed origin.");
    }
}