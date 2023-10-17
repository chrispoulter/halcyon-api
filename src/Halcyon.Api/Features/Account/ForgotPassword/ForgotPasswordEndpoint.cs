using Halcyon.Api.Data;
using Halcyon.Api.Services.Email;
using Halcyon.Api.Services.Email.Templates;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Account.ForgotPassword
{
    public class ForgotPasswordEndpoint : IEndpoint
    {
        public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
        {
            builder.MapPut("/account/forgot-password", HandleAsync)
                .AddFluentValidationAutoValidation()
                .WithTags("Account")
                .Produces(StatusCodes.Status200OK)
                .ProducesValidationProblem();

            return builder;
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
