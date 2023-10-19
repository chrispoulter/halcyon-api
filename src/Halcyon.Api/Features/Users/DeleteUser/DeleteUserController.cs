using Halcyon.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.DeleteUser;

public class DeleteUserController : BaseController
{
    private readonly HalcyonDbContext _context;

    public DeleteUserController(HalcyonDbContext context)
    {
        _context = context;
    }

    [HttpDelete("/user/{id}")]
    [Authorize(Policy = "UserAdministratorPolicy")]
    [Tags("User")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Index(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
    {
        var user = await _context.Users
           .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
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

        if (user.Id == CurrentUserId)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Cannot delete currently logged in user."
            );
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return Ok(new UpdateResponse { Id = user.Id });
    }
}
