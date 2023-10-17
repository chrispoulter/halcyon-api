using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Security.Claims;

namespace Halcyon.Api.Features.Manage.UpdateProfile
{
    public class UpdateProfileEndpoint : IEndpoint
    {
        public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
        {
            builder.MapPut("/manage", HandleAsync)
                .RequireAuthorization()
                .AddFluentValidationAutoValidation()
                .WithTags("Manage")
                .Produces<UpdateResponse>()
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status409Conflict);

            return builder;
        }

        public static async Task<IResult> HandleAsync(
            UpdateProfileRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext)
        {
            var currentUserId = currentUser.GetUserId();

            var user = await dbContext.Users
                  .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user is null || user.IsLockedOut)
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
