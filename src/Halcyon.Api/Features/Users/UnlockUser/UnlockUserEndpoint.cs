using Halcyon.Api.Data;
using Halcyon.Api.Data.Users;
using Halcyon.Api.Services.Authorization;
using Halcyon.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.UnlockUser;

public class UnlockUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/user/{id}/unlock", HandleAsync)
            .RequireRole(Roles.SystemAdministrator, Roles.UserAdministrator)
            .WithTags(Tags.Users)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromBody] UpdateRequest request,
        HalcyonDbContext dbContext,
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

        user.IsLockedOut = false;
        user.Raise(new UserUpdatedDomainEvent(user.Id));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
