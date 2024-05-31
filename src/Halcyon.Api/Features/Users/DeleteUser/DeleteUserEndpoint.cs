using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.DeleteUser;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/user/{id}", HandleAsync)
            .RequireAuthorization(nameof(Policy.IsUserAdministrator))
            .WithTags(Tags.Users)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] UpdateRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

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
                title: "Cannot delete currently logged in user."
            );
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new MessageEvent { Type = MessageType.UserDeleted, Id = user.Id },
            cancellationToken
        );

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
