using FluentValidation;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Halcyon.Api.Features.Users.CreateUser
{
    public static class CreateUserEndpoint
    {
        public static WebApplication MapCreateUserEndpoint(this WebApplication app)
        {
            app.MapPost("/user", HandleAsync)
                .RequireAuthorization("UserAdministratorPolicy")
                .WithTags("Users")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            CreateUserRequest request,
            IValidator<CreateUserRequest> validator,
            ClaimsPrincipal currentUser,
            HalcyonDbContext dbContext,
            IHashService hashService)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

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

            var password = hashService.GenerateHash(request.Password);
            var user = (request, password).Adapt<User>();

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
