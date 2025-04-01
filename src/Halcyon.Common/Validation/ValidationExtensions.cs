using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Halcyon.Common.Validation;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder AddValidationFilter<T>(this RouteHandlerBuilder builder)
        where T : class, new()
    {
        return builder.AddEndpointFilter<RouteHandlerBuilder, ValidationFilter<T>>();
    }
}
