using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.LockUser;

public class LockUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/user/{id}/lock", HandleAsync)
            .RequireAuthorization("UserAdministratorPolicy")
            .WithTags("Users")
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    internal static async Task<IResult> HandleAsync(
        int id,
        [FromBody] UpdateRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext)
    {
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

        if (user.Id == currentUser.Id)
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
