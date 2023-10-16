using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.GetUser
{
    public class GetUserEndpoint : IEndpoint
    {
        public WebApplication MapEndpoint(WebApplication app)
        {
            app.MapGet("/user/{id}", HandleAsync)
                .RequireAuthorization("UserAdministratorPolicy")
                .WithTags("Users")
                .Produces<GetUserResponse>()
                .ProducesProblem(StatusCodes.Status404NotFound);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            int id,
            HalcyonDbContext dbContext)
        {
            var user = await dbContext.Users
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            var result = user.Adapt<GetUserResponse>();

            return Results.Ok(result);
        }
    }
}
