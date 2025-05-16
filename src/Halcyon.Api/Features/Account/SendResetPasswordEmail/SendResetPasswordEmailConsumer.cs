using System.Reflection;
using FluentEmail.Core;
using Halcyon.Api.Common.Messaging;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.ForgotPassword;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer(HalcyonDbContext dbContext, IFluentEmail fluentEmail)
    : IMessageConsumer<ResetPasswordRequestedEvent>
{
    public async Task Consume(
        ResetPasswordRequestedEvent message,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext
            .Users.Where(u => u.Id == message.UserId && u.PasswordResetToken != null)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return;
        }

        var assembly = Assembly.GetExecutingAssembly();

        await fluentEmail
            .To(user.EmailAddress)
            .Subject("Reset Password // Halcyon")
            .UsingTemplateFromEmbedded(
                "Halcyon.Api.Features.Account.SendResetPasswordEmail.ResetPasswordEmail.html",
                new { user.PasswordResetToken },
                assembly
            )
            .SendAsync(cancellationToken);
    }
}
