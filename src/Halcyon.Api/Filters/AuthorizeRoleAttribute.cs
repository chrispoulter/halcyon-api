using Halcyon.Api.Data;
using Microsoft.AspNetCore.Authorization;

namespace Halcyon.Api.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params Role[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
