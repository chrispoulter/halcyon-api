using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ResetPassword;

public class ResetPasswordController : BaseController
{
    private readonly HalcyonDbContext _dbContext;

    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordController(
        HalcyonDbContext dbContext,
        IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpPut("/account/reset-password")]
    [Tags("Account")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index(ResetPasswordRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (
            user is null
            || user.IsLockedOut
            || request.Token != user.PasswordResetToken)
        {
            return Problem(
              statusCode: StatusCodes.Status400BadRequest,
              title: "Invalid token."
          );
        }

        user.Password = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;

        await _dbContext.SaveChangesAsync();

        return Ok(new UpdateResponse { Id = user.Id });
    }
}
