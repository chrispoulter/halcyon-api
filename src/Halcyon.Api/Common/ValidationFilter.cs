using FluentValidation;

namespace Halcyon.Api.Common;

public class ValidationFilter : IEndpointFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null)
            {
                return Results.Problem(
                   statusCode: StatusCodes.Status400BadRequest,
                   title: "A non-empty request body is required."
               );
            }

            var argumentType = argument.GetType();

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Select(e => e.ErrorMessage).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }
        }

        return await next(context);
    }
}
