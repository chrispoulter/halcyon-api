using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace Halcyon.Api.Features.Manage.ChangePassword
{
    public static class ChangePasswordEndpoint
    {
        public static WebApplication MapChangePasswordEndpoint(this WebApplication app)
        {
            app.MapPut("/manage/change-password", HandleAsync)
                .RequireAuthorization()
                .WithTags("Manage")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            ChangePasswordRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext,
            IHashService hashService)
        {
            var (isValid, errors) = await MiniValidator.TryValidateAsync(request);
            if (!isValid)
            {
                return Results.ValidationProblem(errors);
            }

            var currentUserId = currentUser.GetUserId();

            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user is null || user.IsLockedOut)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request.Version is not null && request.Version != user.Version)
            {
                return Results.Problem(
                     statusCode: StatusCodes.Status409Conflict,
                     title: "Data has been modified since entities were loaded."
                 );
            }

            if (user.Password is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Incorrect password."
                );
            }

            var verified = hashService.VerifyHash(request.CurrentPassword, user.Password);

            if (!verified)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Incorrect password."
                );
            }

            user.Password = hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
