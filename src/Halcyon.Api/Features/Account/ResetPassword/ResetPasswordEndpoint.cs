﻿using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace Halcyon.Api.Features.Account.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/reset-password", HandleAsync)
            .AddValidationFilter<ResetPasswordRequest>()
            .WithTags(EndpointTag.Account)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        HalcyonDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.EmailAddress == request.EmailAddress,
            cancellationToken
        );

        if (user is null || user.IsLockedOut || request.Token != user.PasswordResetToken)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid token."
            );
        }

        user.Password = BC.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
