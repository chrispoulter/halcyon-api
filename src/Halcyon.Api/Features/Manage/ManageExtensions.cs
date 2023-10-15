using Halcyon.Api.Features.Manage.ChangePassword;
using Halcyon.Api.Features.Manage.DeleteProfile;
using Halcyon.Api.Features.Manage.GetProfile;
using Halcyon.Api.Features.Manage.UpdateProfile;

namespace Halcyon.Api.Features.Manage
{
    public static class ManageEndpointExtensions
    {
        public static WebApplication MapManageEndpoints(this WebApplication app)
        {
            app.MapGetProfileEndpoint();
            app.MapUpdateProfileEndpoint();
            app.MapChangePasswordEndpoint();
            app.MapDeleteProfileEndpoint();

            return app;
        }
    }
}
