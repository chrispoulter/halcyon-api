using FluentValidation;

namespace Halcyon.Api.Features
{
    public class RequestValidationFilter<T> : IEndpointFilter
    {
        private readonly IValidator<T> _validator;

        public RequestValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var request = context.GetArgument<T>(0);

            if (request is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Request can not be null."
                );
            }

            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
