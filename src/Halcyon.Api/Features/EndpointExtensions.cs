using System.Security.Claims;

namespace Halcyon.Api.Features
{
    public static class EndpointExtensions
    {
        public static int GetUserId(this ClaimsPrincipal currentUser)
            => int.Parse(currentUser.Identity.Name);
    }
}
