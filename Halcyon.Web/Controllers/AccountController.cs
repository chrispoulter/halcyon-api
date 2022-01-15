using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Models.Account;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Email;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
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
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (existing != null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.DUPLICATE_USER,
                    Message = $"User name \"{model.EmailAddress}\" is already taken."
                });
            }

            var user = new User
            {
                EmailAddress = model.EmailAddress,
                Password = _hashService.GenerateHash(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.Value.ToUniversalTime()
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.USER_REGISTERED,
                Message = "User successfully registered.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("forgotpassword")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (user != null && !user.IsLockedOut)
            {
                user.PasswordResetToken = Guid.NewGuid().ToString();

                await _context.SaveChangesAsync();

                var message = new EmailMessage
                {
                    Template = EmailTemplate.RESET_PASSWORD
                };

                message.To.Add(user.EmailAddress);
                message.Data.Add("SiteUrl", $"{Request.Scheme}://{Request.Host}");
                message.Data.Add($"PasswordResetUrl", $"{Request.Scheme}://{Request.Host}/reset-password/{user.PasswordResetToken}");

                await _emailService.SendEmailAsync(message);
            }

            return Ok(new ApiResponse
            {
                Code = InternalStatusCode.FORGOT_PASSWORD,
                Message = "Instructions as to how to reset your password have been sent to you via email."
            });
        }

        [HttpPut("resetpassword")]
        [ProducesResponseType(typeof(ApiResponse<UserUpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (
                user == null
                || user.IsLockedOut
                || !model.Token.Equals(user.PasswordResetToken, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.INVALID_TOKEN,
                    Message = "Invalid token."
                });
            }

            user.Password = _hashService.GenerateHash(model.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserUpdatedResponse>
            {
                Code = InternalStatusCode.PASSWORD_RESET,
                Message = "Your password has been reset.",
                Data = { Id = user.Id }
            });
        }
    }
}
