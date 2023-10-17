using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Account.ResetPassword
{
    public class ResetPasswordEndpoint : IEndpoint
    {
        public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
        {
            builder.MapPut("/account/reset-password", HandleAsync)
                .AddFluentValidationAutoValidation()
                .WithTags("Account")
                .Produces<UpdateResponse>()
                .ProducesValidationProblem();

            return builder;
        }

        public static async Task<IResult> HandleAsync(
            ResetPasswordRequest request,
            HalcyonDbContext dbContext,
            IHashService hashService)
        {
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (
                user is null
                || user.IsLockedOut
                || request.Token != user.PasswordResetToken)
            {
                return Results.Problem(
                  statusCode: StatusCodes.Status400BadRequest,
                  title: "Invalid token."
              );
            }

            user.Password = hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await dbContext.SaveChangesAsync();

            return Results.Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
