using FluentValidation;

namespace Halcyon.Api.Common.Validation;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class, new()
{
    public async ValueTask<object> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        var result = await validator.ValidateAsync(request ?? new T());

        return result.IsValid
            ? await next(context)
            : Results.ValidationProblem(result.ToDictionary());
    }
}
