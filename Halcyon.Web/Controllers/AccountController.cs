using Halcyon.Web.Data;
using Halcyon.Web.Models.Account;
using Halcyon.Web.Models.User;
using Halcyon.Web.Services.Email;
using Halcyon.Web.Services.Password;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordService _hashService;

        private readonly IEmailService _emailService;

        public AccountController(
            HalcyonDbContext context,
            IPasswordService hashService,
            IEmailService emailService)
        {
            _context = context;
            _hashService = hashService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (existing != null)
            {
                return BadRequest($"User name \"{model.EmailAddress}\" is already taken.");
            }

            var user = new User
            {
                EmailAddress = model.EmailAddress,
                Password = _hashService.GenerateHash(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.ToUniversalTime()
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var result = new UserCreatedResponse
            {
                UserId = user.Id
            };

            return Ok(result, "User successfully registered.");
        }

        [HttpPut("forgotpassword")]
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
                    Template = EmailTemplate.ForgotPassword
                };

                message.To.Add(user.EmailAddress);
                message.Data.Add("SiteUrl", $"{Request.Scheme}://{Request.Host}");
                message.Data.Add($"PasswordResetUrl", $"{Request.Scheme}://{Request.Host}/resetpassword/{user.PasswordResetToken}");

                await _emailService.SendEmailAsync(message);
            }

            return Ok("Instructions as to how to reset your password have been sent to you via email.");
        }

        [HttpPut("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (
                user == null
                || user.IsLockedOut
                || !model.Token.Equals(user.PasswordResetToken, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Invalid token.");
            }

            user.Password = _hashService.GenerateHash(model.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok("Your password has been reset.");
        }
    }
}
