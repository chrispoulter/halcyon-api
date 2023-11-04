using Carter;
using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.SearchUsers;

public class SearchUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/user", HandleAsync)
            .RequireAuthorization("UserAdministratorPolicy")
            .AddEndpointFilter<ValidationFilter>()
            .WithTags("Users")
            .Produces<SearchUsersResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    internal static async Task<IResult> HandleAsync(
        [AsParameters] SearchUsersRequest request,
        HalcyonDbContext dbContext)
    {
        var query = dbContext.Users
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

        return Results.Ok(new SearchUsersResponse()
        {
            Items = users,
            HasNextPage = request.Page < pageCount,
            HasPreviousPage = request.Page > 1
        });
    }
}
