using Halcyon.Web.Services.Date;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Filters
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
            if (value is not DateTime date)
            {
                return ValidationResult.Success;
            }

            var dateService = validationContext.GetService<IDateService>();

            if (date < dateService.UtcNow)
            { 
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
