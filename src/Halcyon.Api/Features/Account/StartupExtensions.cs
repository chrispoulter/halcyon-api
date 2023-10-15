using Halcyon.Api.Features.Account.ForgotPassword;
using Halcyon.Api.Features.Account.Register;
using Halcyon.Api.Features.Account.ResetPassword;

namespace Halcyon.Api.Features.Account
{
    public static class StartupExtensions
    {
        public static WebApplication MapAccountEndpoints(this WebApplication app)
        {
            app.MapRegisterEndpoint();
            app.MapForgotPasswordEndpoint();
            app.MapResetPasswordEndpoint();

            return app;
        }
    }
}
