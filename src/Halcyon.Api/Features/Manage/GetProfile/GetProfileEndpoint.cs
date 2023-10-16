using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Halcyon.Api.Features.Manage.GetProfile
{
    public class GetProfileEndpoint : IEndpoint
    {
        public WebApplication MapEndpoint(WebApplication app)
        {
            app.MapGet("/manage", HandleAsync)
                .RequireAuthorization()
                .WithTags("Manage")
                .Produces<GetProfileResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext)
        {
            var currentUserId = currentUser.GetUserId();

            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

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
}
