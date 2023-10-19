using Halcyon.Api.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.UpdateProfile;

public class UpdateProfileController : BaseController
{
    private readonly HalcyonDbContext _context;

    public UpdateProfileController(HalcyonDbContext context)
    {
        _context = context;
    }

    [HttpPut("/manage")]
    [Authorize]
    [Tags("Manage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Index(UpdateProfileRequest request)
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

        if (!request.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing is not null)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "User name is already taken."
                );
            }
        }

        request.Adapt(user);

        await _context.SaveChangesAsync();

        return Ok(new UpdateResponse { Id = user.Id });
    }
}
