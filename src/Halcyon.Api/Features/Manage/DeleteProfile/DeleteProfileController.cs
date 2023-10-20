using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.DeleteProfile;

public class DeleteProfileController : BaseController
{
    private readonly HalcyonDbContext _dbContext;

    public DeleteProfileController(HalcyonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpDelete("/manage")]
    [Authorize]
    [Tags("Manage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Index([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

        if (user is null || user.IsLockedOut)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        if (request?.Version is not null && request.Version != user.Version)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Data has been modified since entities were loaded."
            );
        }

        _dbContext.Users.Remove(user);

        await _dbContext.SaveChangesAsync();

        return Ok(new UpdateResponse { Id = user.Id });
    }
}
