﻿using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.UpdateProfile;

public class UpdateProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/manage", HandleAsync)
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter>()
            .WithTags("Manage")
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    internal static async Task<IResult> HandleAsync(
        UpdateProfileRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
              .FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken);

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

        if (!request.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
        {
            var existing = await dbContext.Users
                .AnyAsync(u => u.EmailAddress == request.EmailAddress, cancellationToken);

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

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
