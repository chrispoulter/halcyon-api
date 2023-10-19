﻿using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordController : BaseController
{
    private readonly HalcyonDbContext _context;

    private readonly IBus _bus;

    public ForgotPasswordController(
        HalcyonDbContext context,
        IBus bus)
    {
        _context = context;
        _bus = bus;
    }

    [HttpPut("/account/forgot-password")]
    [Tags("Account")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index(ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (user is not null && !user.IsLockedOut)
        {
            user.PasswordResetToken = Guid.NewGuid();

            await _context.SaveChangesAsync();

            var message = new SendResetPasswordEmailEvent
            {
                To = user.EmailAddress,
                PasswordResetToken = user.PasswordResetToken,
                SiteUrl = request.SiteUrl,
            };

            await _bus.Publish(message);
        }

        return Ok();
    }
}