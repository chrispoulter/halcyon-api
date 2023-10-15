using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.GetProfile
{
    public static class UpdateProfileEndpoint
    {
        public static WebApplication MapGetProfileEndpoint(this WebApplication app)
        {
            app.MapGet("/manage", HandleAsync)
                .WithTags("Manage")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            int currentUserId,
            UpdateRequest request,
            HalcyonDbContext dbContext)
        {
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
