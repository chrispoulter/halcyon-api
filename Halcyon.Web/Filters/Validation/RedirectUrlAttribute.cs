using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Halcyon.Web.Filters.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RedirectUrlAttribute : ValidationAttribute
    {
        public RedirectUrlAttribute()
        {
            ErrorMessage = "The {0} field is not an allowed origin.";
        }

        override protected ValidationResult IsValid(object value, ValidationContext context)
        {
            var url = value as string;

            if (string.IsNullOrEmpty(url))
            {
                return ValidationResult.Success;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            var origin = uri.GetLeftPart(UriPartial.Authority);

            var corsOptions = context.GetService<IOptions<CorsOptions>>();

            var corsPolicy = corsOptions.Value.GetPolicy(corsOptions.Value.DefaultPolicyName);

            if (corsPolicy is null || corsPolicy.AllowAnyOrigin)
            {
                return ValidationResult.Success;
            }

            var isAllowed = corsPolicy
                .Origins
                .Any(pattern => Regex.IsMatch(origin, Regex.Escape(pattern).Replace("\\*", "(.+)"), RegexOptions.IgnoreCase));

            if (!isAllowed)
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}