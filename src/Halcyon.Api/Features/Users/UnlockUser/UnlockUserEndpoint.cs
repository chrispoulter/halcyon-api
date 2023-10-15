using Halcyon.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Users.UnlockUser
{
    public static class UnlockUserEndpoint
    {
        public static WebApplication MapUnlockUserEndpoint(this WebApplication app)
        {
            app.MapPut("/user/{id}/unlock", HandleAsync)
                .RequireAuthorization("UserAdministratorPolicy")
                .AddFluentValidationAutoValidation()
                .WithTags("Users")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            int id,
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request,
            HalcyonDbContext dbContext)
        {
            var user = await dbContext.Users
                  .FirstOrDefaultAsync(u => u.Id == id);

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

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
