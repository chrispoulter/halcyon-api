using Halcyon.Api.Services.Date;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Filters
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PastAttribute : ValidationAttribute
    {
        public PastAttribute()
        {
            ErrorMessage = "The {0} field must be in the past.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not DateOnly date)
            {
                return ValidationResult.Success;
            }

            var dateService = validationContext.GetService<IDateService>();

            if (date < DateOnly.FromDateTime(dateService.UtcNow))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
