using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Account.ForgotPassword;

public class ForgotPasswordController : BaseController
{
    private readonly HalcyonDbContext _dbContext;

    private readonly IPublishEndpoint _publishEndpoint;

    public ForgotPasswordController(
        HalcyonDbContext dbContext,
        IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPut("/account/forgot-password")]
    [Tags("Account")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index(ForgotPasswordRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (user is not null && !user.IsLockedOut)
        {
            user.PasswordResetToken = Guid.NewGuid();

            await _dbContext.SaveChangesAsync();

            var message = new SendResetPasswordEmailEvent
            {
                To = user.EmailAddress,
                PasswordResetToken = user.PasswordResetToken,
                SiteUrl = request.SiteUrl,
            };

            await _publishEndpoint.Publish(message);
        }

        return Ok();
    }
}
