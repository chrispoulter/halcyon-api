using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ResetPassword
{
    public class ResetPasswordController : ControllerBase
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordController(
            HalcyonDbContext context,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPut("/account/reset-password")]
        [Tags("Account")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Index(ResetPasswordRequest request)
        {
            var user = await _context.Users
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

            user.Password = _passwordHasher.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
