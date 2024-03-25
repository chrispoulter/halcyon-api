using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/user", HandleAsync)
            .RequireAuthorization("UserAdministratorPolicy")
            .AddEndpointFilter<ValidationFilter>()
            .WithTags("Users")
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal static async Task<IResult> HandleAsync(
        CreateUserRequest request,
        CurrentUser currentUser,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken = default
    )
    {
        var existing = await dbContext.Users.AnyAsync(
            u => u.EmailAddress == request.EmailAddress,
            cancellationToken
        );

        if (existing)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "User name is already taken."
            );
        }

        var user = request.Adapt<User>();
        user.Password = passwordHasher.HashPassword(request.Password);

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
