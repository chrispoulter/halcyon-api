using Halcyon.Api.Common.Infrastructure;
using Halcyon.Api.Common.Validation;
using Halcyon.Api.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/forgot-password", HandleAsync)
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(Tags.Account)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        HalcyonDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken = default
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            u => u.EmailAddress == request.EmailAddress,
            cancellationToken
        );

        if (user is not null && !user.IsLockedOut)
        {
            user.PasswordResetToken = Guid.NewGuid();

            await dbContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(
                new ResetPasswordRequestedEvent(user.Id),
                cancellationToken
            );
        }

        return Results.Ok();
    }
}
