using Halcyon.Api.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Manage.GetProfile;

public class GetProfileController : BaseController
{
    private readonly HalcyonDbContext _dbContext;

    public GetProfileController(HalcyonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("/manage")]
    [Authorize]
    [Tags("Manage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Index()
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

        if (user is null || user.IsLockedOut)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var result = user.Adapt<GetProfileResponse>();

        return Ok(result);
    }
}
