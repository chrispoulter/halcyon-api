using Halcyon.Api.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.GetUser;

public class GetUserController : BaseController
{
    private readonly HalcyonDbContext _context;

    public GetUserController(HalcyonDbContext context)
    {
        _context = context;
    }

    [HttpGet("/user/{id}")]
    [Authorize(Policy = "UserAdministratorPolicy")]
    [Tags("User")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Index(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "User not found."
            );
        }

        var result = user.Adapt<GetUserResponse>();

        return Ok(result);
    }
}
