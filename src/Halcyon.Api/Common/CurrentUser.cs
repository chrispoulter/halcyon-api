namespace Halcyon.Api.Common;

public record CurrentUser(int Id)
{
    public static ValueTask<CurrentUser> BindAsync(HttpContext httpContext)
    {
        var id = int.Parse(httpContext.User.Identity.Name);
        return ValueTask.FromResult(new CurrentUser(id));
    }
}