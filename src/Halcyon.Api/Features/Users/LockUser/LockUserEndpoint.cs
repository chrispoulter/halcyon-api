using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Halcyon.Api.Features.Users.LockUser
{
    public static class LockUserEndpoint
    {
        public static WebApplication MapLockUserEndpoint(this WebApplication app)
        {
            app.MapPut("/user/{id}/lock", HandleAsync)
                .RequireAuthorization()
                .WithTags("Users")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            int id,
            UpdateRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext)
        {
            var currentUserId = currentUser.GetUserId();

            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            if (user.Id == currentUserId)
            {
                return Results.Problem(
                     statusCode: StatusCodes.Status400BadRequest,
                     title: "Cannot lock currently logged in user."
                 );
            }
            user.IsLockedOut = true;

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
