﻿using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Halcyon.Api.Services.Hash;
using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/user", HandleAsync)
            .RequireAuthorization(nameof(Policy.IsUserAdministrator))
            .AddEndpointFilter<ValidationFilter>()
            .WithTags(Tags.Users)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IPublishEndpoint publishEndpoint,
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

        await publishEndpoint.Publish(
            new MessageEvent { Type = MessageType.UserCreated, Id = user.Id },
            cancellationToken
        );

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
