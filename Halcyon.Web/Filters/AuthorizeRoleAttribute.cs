using Halcyon.Web.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Halcyon.Web.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params Roles[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.GetType()
                   .GetMember(r.ToString())
                   .First()
                   .GetCustomAttribute<DisplayAttribute>()
                   .Name));
        }
    }
}
