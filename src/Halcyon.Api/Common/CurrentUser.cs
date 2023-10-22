namespace Halcyon.Api.Common;

public class CurrentUser
{
    public int Id { get; set; }
 
    public static ValueTask<CurrentUser> BindAsync(HttpContext httpContext)
    {
        if (!int.TryParse(httpContext.User.Identity.Name, out var id))
        { 
            return ValueTask.FromResult<CurrentUser>(null);
        }

        return ValueTask.FromResult(new CurrentUser { Id = id });
    }
}