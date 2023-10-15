using FluentValidation;

namespace Halcyon.Api.Features
{
    public static class RequestValidationFilterExtensions
    {
        public static RouteHandlerBuilder AddValidationFilter<T>(this RouteHandlerBuilder builder)
            => builder.AddEndpointFilter<RequestValidationFilter<T>>();
    }

    public class RequestValidationFilter<T> : IEndpointFilter
    {
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

            var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
