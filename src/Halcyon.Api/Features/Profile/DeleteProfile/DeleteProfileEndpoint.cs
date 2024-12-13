using Halcyon.Api.Data;
using Halcyon.Api.Data.Users;
using Halcyon.Api.Services.Authentication;
using Halcyon.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Profile.DeleteProfile;

public class DeleteProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("/profile", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
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
        user.Raise(new UserDeletedDomainEvent(user.Id));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
