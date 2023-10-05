﻿using Halcyon.Web.Data;
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
        [ProducesResponseType(typeof(UpdatedResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing is not null)
            {
                return Problem(
                  statusCode: (int)HttpStatusCode.BadRequest,
                  title: "User name is already taken."
              );
            }

            var user = new User
            {
                EmailAddress = request.EmailAddress,
                Password = _hashService.GenerateHash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth.Value.ToUniversalTime()
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new UpdatedResponse { Id = user.Id });
        }

        [HttpPut("forgot-password")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (user is not null && !user.IsLockedOut)
            {
                user.PasswordResetToken = Guid.NewGuid();

                await _context.SaveChangesAsync();

                var message = new EmailMessage
                {
                    Template = EmailTemplate.RESET_PASSWORD,
                    To = new()
                    {
                        user.EmailAddress
                    },
                    Data = new()
                    {
                        { "SiteUrl", request.SiteUrl },
                        { "PasswordResetUrl", $"{request.SiteUrl}/reset-password/{user.PasswordResetToken}" }
                    }
                };

                await _emailService.SendEmailAsync(message);
            }

            return Ok();
        }

        [HttpPut("reset-password")]
        [ProducesResponseType(typeof(UpdatedResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (
                user is null
                || user.IsLockedOut
                || request.Token != user.PasswordResetToken)
            {
                return Problem(
                  statusCode: (int)HttpStatusCode.BadRequest,
                  title: "Invalid token."
              );
            }

            user.Password = _hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new UpdatedResponse { Id = user.Id });
        }
    }
}
