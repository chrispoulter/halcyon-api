using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Security.Claims;

namespace Halcyon.Api.Features.Users.CreateUser
{
    public class CreateUserEndpoint : IEndpoint
    {
        public static IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/user", HandleAsync)
                .RequireAuthorization("UserAdministratorPolicy")
                .AddFluentValidationAutoValidation()
                .WithTags("Users")
                .Produces<UpdateResponse>()
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return endpoints;
        }

        public static async Task<IResult> HandleAsync(
            CreateUserRequest request,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext,
            IPasswordHasher passwordHasher)
        {
            var currentUserId = currentUser.GetUserId();

            var existing = await dbContext.Users
               .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing is not null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "User name is already taken."
                );
            }

            var user = request.Adapt<User>();
            user.Password = passwordHasher.GenerateHash(request.Password);

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
