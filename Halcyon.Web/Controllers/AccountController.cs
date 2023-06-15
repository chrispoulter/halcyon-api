using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Models.Account;
using Halcyon.Web.Services.Email;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class AccountController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        private readonly IEmailService _emailService;

        public AccountController(
            HalcyonDbContext context,
            IHashService hashService,
            IEmailService emailService)
        {
            _context = context;
            _hashService = hashService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing != null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = "DUPLICATE_USER",
                    Message = $"User name \"{request.EmailAddress}\" is already taken."
                });
            }

            var user = new User
            {
                EmailAddress = request.EmailAddress,
                Password = _hashService.GenerateHash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth.Value.ToUniversalTime(),
                Version = Guid.NewGuid()
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = "USER_REGISTERED",
                Message = "User successfully registered.",
                Data = new UpdatedResponse { Id = user.Id }
            });
        }

        [HttpPut("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (user != null && !user.IsLockedOut)
            {
                user.PasswordResetToken = Guid.NewGuid().ToString();

                await _context.SaveChangesAsync();

                var message = new EmailMessage
                {
                    Template = EmailTemplate.RESET_PASSWORD
                };

                message.To.Add(user.EmailAddress);
                message.Data.Add("SiteUrl", request.SiteUrl);
                message.Data.Add($"PasswordResetUrl", $"{request.SiteUrl}/reset-password/{user.PasswordResetToken}");

                await _emailService.SendEmailAsync(message);
            }

            return Ok(new ApiResponse
            {
                Code = "FORGOT_PASSWORD",
                Message = "Instructions as to how to reset your password have been sent to you via email."
            });
        }

        [HttpPut("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (
                user == null
                || user.IsLockedOut
                || !request.Token.Equals(user.PasswordResetToken, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest(new ApiResponse
                {
                    Code = "INVALID_TOKEN",
                    Message = "Invalid token."
                });
            }

            user.Password = _hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = "PASSWORD_RESET",
                Message = "Your password has been reset.",
                Data = new UpdatedResponse { Id = user.Id }
            });
        }
    }
}
