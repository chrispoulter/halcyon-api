using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.DeleteUser;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/user/{id}", HandleAsync)
            .RequireAuthorization("UserAdministratorPolicy")
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
        IHubContext<MessageHub, IMessageClient> messageHubContext,
        TimeProvider timeProvider,
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

        var groups = new string[]
        {
            "SYSTEM_ADMINISTRATOR",
            "USER_ADMINISTRATOR",
            $"USER_{user.Id}"
        };

        var message = new Message
        {
            Content = $"User {user.EmailAddress} has been deleted.",
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedBy = currentUser.Id,
        };

        messageHubContext.Clients.Groups(groups).ReceiveMessage(message, cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
