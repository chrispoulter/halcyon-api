using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.GetProfile;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/manage", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Manage)
            .Produces<GetProfileResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        IHubContext<MessagingHub, IMessagingClient> context,
        CancellationToken cancellationToken = default
    )
    {
        await context.Clients.All.ReceiveMessage($"[ALL] Getting Profile {currentUser.Id}");

        //await context.Clients.User(currentUser.Id.ToString()).ReceiveNotification($"[USER] Getting Profile {currentUser.Id}");

        var user = await dbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken);

        if (user is null || user.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var result = user.Adapt<GetProfileResponse>();

        return Results.Ok(result);
    }
}
