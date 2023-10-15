using FluentValidation;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Token
{
    public static class TokenEndpoint
    {
        public static WebApplication MapTokenEndpoint(this WebApplication app)
        {
            app.MapPost("/token", HandleAsync)
                .AddFluentValidationAutoValidation()
                .WithTags("Token")
                .Produces<Services.Jwt.Token>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);

            return app;
        }

        public static async Task<IResult> HandleAsync(
            TokenRequest request,
            IValidator<TokenRequest> validator,
            HalcyonDbContext dbContext,
            IHashService hashService,
            IJwtService jwtService)
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

            var verified = hashService.VerifyHash(request.Password, user.Password);

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

            var result = jwtService.CreateToken(user);

            return Results.Ok(result);
        }
    }
}
