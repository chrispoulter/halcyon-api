using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Halcyon.Api.Features.Account.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/account/reset-password", HandleAsync)
            .AddFluentValidationAutoValidation()
            .WithTags("Account")
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    public static async Task<IResult> HandleAsync(
        ResetPasswordRequest request,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher)
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

        user.Password = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await dbContext.SaveChangesAsync();

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
