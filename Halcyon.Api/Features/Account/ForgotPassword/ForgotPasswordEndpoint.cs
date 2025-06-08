using System.Reflection;
using FluentEmail.Core;
using Halcyon.Api.Common.Infrastructure;
using Halcyon.Api.Common.Validation;
using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/account/forgot-password", HandleAsync)
            .AddValidationFilter<ForgotPasswordRequest>()
            .WithTags(Tags.Account);
    }

    private static async Task<IResult> HandleAsync(
        ForgotPasswordRequest request,
        HalcyonDbContext dbContext,
        IFluentEmail fluentEmail,
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

            var assembly = Assembly.GetExecutingAssembly();

            await fluentEmail
                .To(user.EmailAddress)
                .Subject("Reset Password // Halcyon")
                .UsingTemplateFromEmbedded(
                    "Halcyon.Api.Features.Account.ForgotPassword.ResetPasswordEmail.html",
                    new { user.PasswordResetToken },
                    assembly
                )
                .SendAsync(cancellationToken);
        }

        return Results.Ok();
    }
}
