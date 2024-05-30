using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.ChangePassword;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/manage/change-password", HandleAsync)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter>()
            .WithTags(Tags.Manage)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        ChangePasswordRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
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

        var verified = passwordHasher.VerifyPassword(request.CurrentPassword, user.Password);

        if (!verified)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Incorrect password."
            );
        }

        user.Password = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        var groups = new string[]
        {
            "SYSTEM_ADMINISTRATOR",
            "USER_ADMINISTRATOR",
            $"USER_{user.Id}"
        };

        var message = new Message
        {
            Content = $"User {user.EmailAddress} has changed password.",
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedBy = currentUser.Id,
        };

        messageHubContext.Clients.Groups(groups).ReceiveMessage(message, cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
