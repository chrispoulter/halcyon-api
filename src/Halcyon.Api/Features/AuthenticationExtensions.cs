using System.Security.Claims;

namespace Halcyon.Api.Features
{
    public static class AuthenticationExtensions
    {
        public static int GetUserId(this ClaimsPrincipal currentUser)
            => int.Parse(currentUser.Identity.Name);
    }
}
