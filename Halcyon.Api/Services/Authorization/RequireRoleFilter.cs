namespace Halcyon.Api.Services.Authorization;

public class RequireRoleFilter(string[] roles) : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return Results.Unauthorized();
        }

        if (!roles.Any(user.IsInRole))
        {
            return Results.Forbid();
        }

        return await next(context);
    }
}
