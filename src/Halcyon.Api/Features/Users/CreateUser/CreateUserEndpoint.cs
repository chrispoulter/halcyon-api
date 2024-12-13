using Halcyon.Api.Data;
using Halcyon.Api.Data.Users;
using Halcyon.Api.Services.Authentication;
using Halcyon.Api.Services.Authorization;
using Halcyon.Api.Services.Infrastructure;
using Halcyon.Api.Services.Validation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/user", HandleAsync)
            .RequireRole(Roles.SystemAdministrator, Roles.UserAdministrator)
            .AddValidationFilter<CreateUserRequest>()
            .WithTags(Tags.Users)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        CreateUserRequest request,
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
        user.Raise(new UserCreatedDomainEvent(user.Id));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
