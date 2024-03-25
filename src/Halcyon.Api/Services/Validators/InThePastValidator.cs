﻿using FluentValidation;

namespace Halcyon.Api.Services.Validators;

public static class InThePastValidator
{
    public static IRuleBuilderOptions<T, DateOnly> InThePast<T>(
        this IRuleBuilder<T, DateOnly> ruleBuilder,
        TimeProvider timeProvider
    ) =>
        ruleBuilder
            .LessThan(DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime))
            .WithMessage("'{PropertyName}' must be in the past.");
}
