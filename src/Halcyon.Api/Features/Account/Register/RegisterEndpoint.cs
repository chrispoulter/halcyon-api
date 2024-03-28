using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.Register;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/register", HandleAsync)
            .AddEndpointFilter<ValidationFilter>()
            .WithTags("Account")
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal static async Task<IResult> HandleAsync(
        RegisterRequest request,
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

        var response = new UpdateResponse(user.Id);
        return Results.Ok(response);
    }
}
