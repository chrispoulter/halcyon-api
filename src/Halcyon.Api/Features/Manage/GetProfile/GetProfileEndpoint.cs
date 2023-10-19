﻿using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Halcyon.Api.Features.Manage.GetProfile;

public class GetProfileEndpoint : IEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/manage", HandleAsync)
            .RequireAuthorization()
            .WithTags("Manage")
            .Produces<GetProfileResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    public static async Task<IResult> HandleAsync(
        ClaimsPrincipal currentUser,
        HalcyonDbContext dbContext)
    {
        var currentUserId = currentUser.GetUserId();

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUserId);

        if (user is null || user.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var result = user.Adapt<GetProfileResponse>();

        return Results.Ok(result);
    }
}
