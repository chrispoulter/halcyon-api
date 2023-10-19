using Halcyon.Api.Data;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.SearchUsers;

public class SearchUsersController : BaseController
{
    private readonly HalcyonDbContext _context;

    public SearchUsersController(HalcyonDbContext context)
    {
        _context = context;
    }

    [HttpGet("/user")]
    [Authorize(Policy = "UserAdministratorPolicy")]
    [Tags("User")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SearchUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index([FromQuery] SearchUsersRequest request)
    {
        var query = _context.Users
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ILike(u.Search, $"%{request.Search}%"));
        }

        var count = await query.CountAsync();

        query = request.Sort switch
        {
            UserSort.EMAIL_ADDRESS_DESC => query.OrderByDescending(r => r.EmailAddress),
            UserSort.EMAIL_ADDRESS_ASC => query.OrderBy(r => r.EmailAddress),
            UserSort.NAME_DESC => query.OrderByDescending(r => r.FirstName).ThenByDescending(r => r.LastName),
            _ => query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName),
        };

        if (request.Page > 1)
        {
            query = query.Skip((request.Page - 1) * request.Size);
        }

        query = query.Take(request.Size);

        var users = await query
            .ProjectToType<SearchUserResponse>()
            .ToListAsync();

        var pageCount = (count + request.Size - 1) / request.Size;

        return Ok(new SearchUsersResponse()
        {
            Items = users,
            HasNextPage = request.Page < pageCount,
            HasPreviousPage = request.Page > 1
        });
    }
}
