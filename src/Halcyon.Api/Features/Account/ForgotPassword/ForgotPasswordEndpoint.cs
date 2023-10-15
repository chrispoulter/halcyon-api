﻿using Halcyon.Api.Data;
using Halcyon.Api.Services.Email;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword
{
    public static class ForgotPasswordEndpoint
    {
        public static WebApplication MapForgotPasswordEndpoint(this WebApplication app)
        {
            app.MapPut("/account/forgot-password", HandleAsync);
            return app;
        }

        public static async Task<IResult> HandleAsync(
            ForgotPasswordRequest request,
            HalcyonDbContext dbContext,
            IBus bus)
        {
            var user = await dbContext.Users
               .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (user is not null && !user.IsLockedOut)
            {
                user.PasswordResetToken = Guid.NewGuid();

                await dbContext.SaveChangesAsync();

                var message = new EmailEvent
                {
                    Template = EmailTemplate.RESET_PASSWORD,
                    To = user.EmailAddress,
                    Data = new()
                    {
                        { "SiteUrl", request.SiteUrl },
                        { "PasswordResetUrl", $"{request.SiteUrl}/reset-password/{user.PasswordResetToken}" }
                    }
                };

                await bus.Publish(message);
            }

            return Results.Ok();
        }
    }
}