using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.UpdateUser;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/user/{id}", HandleAsync)
            .RequireAuthorization(nameof(Policy.IsUserAdministrator))
            .AddEndpointFilter<ValidationFilter>()
            .WithTags(Tags.Users)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        UpdateUserRequest request,
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

        if (request.Version is not null && request.Version != user.Version)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Data has been modified since entities were loaded."
            );
        }

        if (
            !request.EmailAddress.Equals(
                user.EmailAddress,
                StringComparison.InvariantCultureIgnoreCase
            )
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
        }

        request.Adapt(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new MessageEvent { Type = MessageType.UserUpdated, Id = user.Id },
            cancellationToken
        );

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
