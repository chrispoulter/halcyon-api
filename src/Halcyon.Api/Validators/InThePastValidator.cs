using FluentValidation;
using Halcyon.Api.Services.Date;

namespace Halcyon.Api.Validators
{
    public static class InThePastValidator
    {
        public static IRuleBuilderOptions<T, DateOnly?> InThePast<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder, IDateService dateService)
            => ruleBuilder
                .LessThan(DateOnly.FromDateTime(dateService.UtcNow))
                .WithMessage("'{PropertyName}' must be in the past.");
    }
}