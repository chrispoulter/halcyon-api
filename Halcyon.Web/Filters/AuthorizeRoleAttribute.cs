using Halcyon.Web.Data;
using Microsoft.AspNetCore.Authorization;

namespace Halcyon.Web.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params Role[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
