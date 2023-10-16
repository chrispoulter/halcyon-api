using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Account.Register
{
    public static class RegisterEndpoint
    {
        public static WebApplication MapRegisterEndpoint(this WebApplication app)
        {
            app.MapPost("/account/register", HandleAsync)
                .AddFluentValidationAutoValidation()
                .WithTags("Account")
                .Produces<UpdateResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            RegisterRequest request,
            HalcyonDbContext dbContext,
            IHashService hashService)
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

            var password = hashService.GenerateHash(request.Password);
            var user = (request, password).Adapt<User>();

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
