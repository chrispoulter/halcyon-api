namespace Halcyon.Api.Services.Validation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder AddValidationFilter<T>(this RouteHandlerBuilder builder)
        where T : class, new()
    {
        return builder.AddEndpointFilter<RouteHandlerBuilder, ValidationFilter<T>>();
    }
}
