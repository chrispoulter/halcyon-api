using Carter;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Security.Claims;

namespace Halcyon.Api.Features.Manage.ChangePassword;

public class ChangePasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/manage/change-password", HandleAsync)
            .RequireAuthorization()
            .AddFluentValidationAutoValidation()
            .WithTags("Manage")
            .Produces<UpdateResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    public static async Task<IResult> HandleAsync(
        ChangePasswordRequest request,
        ClaimsPrincipal currentUser,
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher)
    {
        var currentUserId = currentUser.GetUserId();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId);

        if (user is null || user.IsLockedOut)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        if (request.Version is not null && request.Version != user.Version)
        {
            return Results.Problem(
                 statusCode: StatusCodes.Status409Conflict,
                 title: "Data has been modified since entities were loaded."
             );
        }

        if (user.Password is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Incorrect password."
            );
        }

        var verified = passwordHasher.VerifyPassword(request.CurrentPassword, user.Password);

        if (!verified)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Incorrect password."
            );
        }

        user.Password = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await dbContext.SaveChangesAsync();

        return Results.Ok(new UpdateResponse { Id = user.Id });
    }
}
