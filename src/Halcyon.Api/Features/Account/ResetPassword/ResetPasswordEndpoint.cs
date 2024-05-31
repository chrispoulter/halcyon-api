using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using Halcyon.Api.Services.Hash;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/reset-password", HandleAsync)
            .AddEndpointFilter<ValidationFilter>()
            .WithTags(Tags.Account)
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher,
        IPublishEndpoint publishEndpoint,
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

        user.Password = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new MessageEvent { Type = MessageType.UserUpdated, Id = user.Id },
            cancellationToken
        );

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
