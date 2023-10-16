using Halcyon.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Halcyon.Api.Features.Manage.DeleteProfile
{
    public class DeleteProfileEndpoint : IEndpoint
    {
        public WebApplication MapEndpoint(WebApplication app)
        {
            app.MapDelete("/manage", HandleAsync)
                .RequireAuthorization()
                .WithTags("Manage")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext)
        {
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

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            dbContext.Users.Remove(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
