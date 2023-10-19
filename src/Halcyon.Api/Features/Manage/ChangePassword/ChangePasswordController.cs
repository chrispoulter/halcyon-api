using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.ChangePassword;

public class ChangePasswordController : BaseController
{
    private readonly HalcyonDbContext _context;

    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordController(HalcyonDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpPut("/manage/change-password")]
    [Authorize]
    [Tags("Manage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Index(ChangePasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

        if (user is null || user.IsLockedOut)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        if (request.Version is not null && request.Version != user.Version)
        {
            return Problem(
                 statusCode: StatusCodes.Status409Conflict,
                 title: "Data has been modified since entities were loaded."
             );
        }

        if (user.Password is null)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Incorrect password."
            );
        }

        var verified = _passwordHasher.VerifyPassword(request.CurrentPassword, user.Password);

        if (!verified)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Incorrect password."
            );
        }

        user.Password = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await _context.SaveChangesAsync();

        return Ok(new UpdateResponse { Id = user.Id });
    }
}
