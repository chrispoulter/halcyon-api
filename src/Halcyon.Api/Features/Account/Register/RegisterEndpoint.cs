﻿using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.Register;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/register", HandleAsync)
            .AddEndpointFilter<ValidationFilter>()
            .WithTags(Tags.Account)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        RegisterRequest request,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IHubContext<MessageHub, IMessageClient> messageHubContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken = default
    )
    {
        var existing = await dbContext.Users.AnyAsync(
            u => u.EmailAddress == request.EmailAddress,
            cancellationToken
        );

        if (existing)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "User name is already taken."
            );
        }

        var user = request.Adapt<User>();
        user.Password = passwordHasher.HashPassword(request.Password);

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        var groups = new string[]
        {
            "SYSTEM_ADMINISTRATOR",
            "USER_ADMINISTRATOR",
            $"USER_{user.Id}"
        };

        var message = new Message
        {
            Content = $"User {user.EmailAddress} has registered.",
            CreatedAt = timeProvider.GetUtcNow(),
            CreatedBy = user.Id,
        };

        messageHubContext.Clients.Groups(groups).ReceiveMessage(message, cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
