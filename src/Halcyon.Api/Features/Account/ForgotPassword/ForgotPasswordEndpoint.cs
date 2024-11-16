using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/forgot-password", HandleAsync)
            .RequireRateLimiting(RateLimiterPolicy.Jwt)
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(EndpointTag.Account)
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

            var message = new SendResetPasswordEmailEvent
            {
                To = user.EmailAddress,
                PasswordResetToken = user.PasswordResetToken,
                SiteUrl = request.SiteUrl,
            };

            await publishEndpoint.Publish(message, cancellationToken);
        }

        return Results.Ok();
    }
}
