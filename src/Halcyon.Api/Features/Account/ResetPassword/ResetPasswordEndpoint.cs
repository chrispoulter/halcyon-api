using Halcyon.Api.Data;
using Halcyon.Api.Models;
using Halcyon.Api.Services.Hash;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ResetPassword
{
    public static class ResetPasswordEndpoint
    {
        public static WebApplication MapResetPasswordEndpoint(this WebApplication app)
        {
            app.MapPut("/account/reset-password", HandleAsync);
            return app;
        }

        public static async Task<IResult> HandleAsync(
            ResetPasswordRequest request,
            HalcyonDbContext dbContext,
            IHashService hashService)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (
                user is null
                || user.IsLockedOut
                || request.Token != user.PasswordResetToken)
            {
                return Results.Problem(
                  statusCode: StatusCodes.Status400BadRequest,
                  title: "Invalid token."
              );
            }

            user.Password = hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
