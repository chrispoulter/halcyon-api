using FluentValidation;
using Halcyon.Api.Core.Authentication;
using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.Login;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/login", HandleAsync)
            .WithTags(Tags.Account)
            .Produces<string>(contentType: "text/plain")
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        IValidator<LoginRequest> validator,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await validator.ValidateAsync(
            request ?? new LoginRequest(),
            cancellationToken
        );

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await dbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress, cancellationToken);

        if (user is null || user.Password is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The credentials provided were invalid."
            );
        }

        var verified = passwordHasher.VerifyPassword(request.Password, user.Password);

        if (!verified)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "The credentials provided were invalid."
            );
        }

        if (user.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "This account has been locked out, please try again later."
            );
        }

        var result = jwtTokenGenerator.GenerateJwtToken(user);

        return Results.Content(result);
    }
}
