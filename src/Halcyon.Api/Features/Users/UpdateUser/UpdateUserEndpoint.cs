using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace Halcyon.Api.Features.Users.UpdateUser
{
    public static class UpdateUserEndpoint
    {
        public static WebApplication MapUpdateUserEndpoint(this WebApplication app)
        {
            app.MapPut("/user/{id}", HandleAsync)
                .RequireAuthorization("UserAdministratorPolicy")
                .WithTags("Users")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            int id,
            UpdateUserRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext)
        {
            var (isValid, errors) = await MiniValidator.TryValidateAsync(request);
            if (!isValid)
            {
                return Results.ValidationProblem(errors);
            }

            var currentUserId = currentUser.GetUserId();

            var user = await dbContext.Users
               .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
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
                    .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

                if (existing is not null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "User name is already taken."
                    );
                }
            }

            request.Adapt(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
