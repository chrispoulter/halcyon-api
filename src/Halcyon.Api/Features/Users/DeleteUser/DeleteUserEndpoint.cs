using Halcyon.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Halcyon.Api.Features.Users.DeleteUser
{
    public class DeleteUserEndpoint : IEndpoint
    {
        public static IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/user/{id}", HandleAsync)
                .RequireAuthorization("UserAdministratorPolicy")
                .WithTags("Users")
                .Produces<UpdateResponse>()
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return endpoints;
        }

        public static async Task<IResult> HandleAsync(
            int id,
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext)
        {
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

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            if (user.Id == currentUserId)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Cannot delete currently logged in user."
                );
            }

            dbContext.Users.Remove(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
