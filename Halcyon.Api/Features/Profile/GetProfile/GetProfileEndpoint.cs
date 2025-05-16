using Halcyon.Api.Common.Authentication;
using Halcyon.Api.Common.Infrastructure;
using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Profile.GetProfile;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile", HandleAsync)
            .RequireAuthorization()
            .WithTags(Tags.Profile)
            .Produces<GetProfileResponse>();
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

        var result = new GetProfileResponse
        {
            Id = user.Id,
            EmailAddress = user.EmailAddress,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Version = user.Version,
        };

        return Results.Ok(result);
    }
}
