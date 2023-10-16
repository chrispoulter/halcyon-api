using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Account.Register
{
    public class RegisterEndpoint : IEndpoint
    {
        public WebApplication MapEndpoint(WebApplication app)
        {
            app.MapPost("/account/register", HandleAsync)
                .AddFluentValidationAutoValidation()
                .WithTags("Account")
                .Produces<UpdateResponse>()
                .ProducesValidationProblem();

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

            var user = request.Adapt<User>();
            user.Password = hashService.GenerateHash(request.Password);

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
