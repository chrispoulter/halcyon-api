using Halcyon.Api.Features.Users.CreateUser;
using Halcyon.Api.Features.Users.DeleteUser;
using Halcyon.Api.Features.Users.GetUser;
using Halcyon.Api.Features.Users.LockUser;
using Halcyon.Api.Features.Users.SearchUsers;
using Halcyon.Api.Features.Users.UnlockUser;
using Halcyon.Api.Features.Users.UpdateUser;

namespace Halcyon.Api.Features.Manage
{
    public static class UserEndpointExtensions
    {
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {
            app.MapSearchUsersEndpoint();
            app.MapGetUserEndpoint();
            app.MapCreateUserEndpoint();
            app.MapUpdateUserEndpoint();
            app.MapLockUserEndpoint();
            app.MapUnlockUserEndpoint();
            app.MapDeleteUserEndpoint();

            return app;
        }
    }
}
