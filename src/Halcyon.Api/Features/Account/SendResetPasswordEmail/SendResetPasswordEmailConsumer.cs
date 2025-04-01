using System.Reflection;
using FluentEmail.Core;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.ForgotPassword;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class SendResetPasswordEmailConsumer(HalcyonDbContext dbContext, IFluentEmail fluentEmail)
    : IConsumer<Batch<ResetPasswordRequestedEvent>>
{
    public async Task Consume(ConsumeContext<Batch<ResetPasswordRequestedEvent>> context)
    {
        var ids = context.Message.Select(m => m.Message.UserId).Distinct();

        var users = await dbContext
            .Users.Where(u => ids.Contains(u.Id) && !u.IsLockedOut && u.PasswordResetToken != null)
            .ToListAsync(context.CancellationToken);

        var assembly = Assembly.GetExecutingAssembly();

        foreach (var user in users)
        {
            await fluentEmail
                .To(user.EmailAddress)
                .Subject("Reset Password // Halcyon")
                .UsingTemplateFromEmbedded(
                    "Halcyon.Api.Features.Account.SendResetPasswordEmail.ResetPasswordEmail.html",
                    new { user.PasswordResetToken },
                    assembly
                )
                .SendAsync(context.CancellationToken);
        }
    }
}
