using System.Security.Claims;

namespace Halcyon.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal currentUser)
            => int.Parse(currentUser.Identity.Name);
    }
}
