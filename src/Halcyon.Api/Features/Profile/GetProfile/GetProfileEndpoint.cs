using Halcyon.Api.Data;
using Halcyon.Api.Services.Authentication;
using Halcyon.Api.Services.Infrastructure;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Profile.GetProfile;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .Produces<GetProfileResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
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
