using Microsoft.AspNetCore.Authorization;

namespace Halcyon.Web.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
