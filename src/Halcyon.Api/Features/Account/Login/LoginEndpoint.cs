﻿using Halcyon.Api.Core.Authentication;
using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Halcyon.Api.Features.Account.Login;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/account/login", HandleAsync)
            .AddValidationFilter<LoginRequest>()
            .WithTags(EndpointTag.Account)
            .Produces<string>(contentType: "text/plain")
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        LoginRequest request,
        HalcyonDbContext dbContext,
        IJwtTokenGenerator jwtTokenGenerator,
        CancellationToken cancellationToken = default
    )
    {
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

        var verified = BC.Verify(request.Password, user.Password);

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
