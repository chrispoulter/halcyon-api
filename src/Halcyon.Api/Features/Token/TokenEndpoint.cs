using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Token;

public class TokenEndpoint : IEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/token", HandleAsync)
            .AddFluentValidationAutoValidation()
            .WithTags("Token")
            .Produces<string>(contentType: "text/plain")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    public static async Task<IResult> HandleAsync(
        TokenRequest request,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

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