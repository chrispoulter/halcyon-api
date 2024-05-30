using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.DeleteProfile;

public class DeleteProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/manage", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Manage)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        IHubContext<MessageHub, IMessageClient> messageHubContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == currentUser.Id,
            cancellationToken
        );

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

        await dbContext.SaveChangesAsync(cancellationToken);

        var groups = new string[]
        {
            "SYSTEM_ADMINISTRATOR",
            "USER_ADMINISTRATOR",
            $"USER_{user.Id}"
        };

        var message = new Message
        {
            Content = $"User {user.EmailAddress} has deleted profile.",
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedBy = currentUser.Id,
        };

        messageHubContext.Clients.Groups(groups).ReceiveMessage(message, cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
