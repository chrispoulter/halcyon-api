using Carter;
using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/forgot-password", HandleAsync)
            .AddEndpointFilter<ValidationFilter>()
            .WithTags("Account")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        HalcyonDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        var user = await dbContext.Users
           .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (user is not null && !user.IsLockedOut)
        {
            user.PasswordResetToken = Guid.NewGuid();

            await dbContext.SaveChangesAsync();

            var message = new SendResetPasswordEmailEvent
            {
                To = user.EmailAddress,
                PasswordResetToken = user.PasswordResetToken,
                SiteUrl = request.SiteUrl,
            };

            await publishEndpoint.Publish(message);
        }

        return Results.Ok();
    }
}
