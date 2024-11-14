using FluentValidation;
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
            .RequireRateLimiting("jwt")
            .WithTags(Tags.Account)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        IValidator<ForgotPasswordRequest> validator,
        HalcyonDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await validator.ValidateAsync(
            request ?? new ForgotPasswordRequest(),
            cancellationToken
        );

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

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
