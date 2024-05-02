using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.SearchUsers;

public class SearchUsersEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/user", HandleAsync)
            .RequireAuthorization("UserAdministratorPolicy")
            .AddEndpointFilter<ValidationFilter>()
            .WithTags(Tags.Users)
            .Produces<SearchUsersResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] SearchUsersRequest request,
        HalcyonDbContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => EF.Functions.ILike(u.Search, $"%{request.Search}%"));
        }

        var count = await query.CountAsync(cancellationToken);

        query = request.Sort switch
        {
            UserSort.EMAIL_ADDRESS_DESC
                => query
                    .OrderByDescending(r => r.EmailAddress)
                    .ThenByDescending(r => r.FirstName)
                    .ThenByDescending(r => r.LastName),

            UserSort.EMAIL_ADDRESS_ASC
                => query
                    .OrderBy(r => r.EmailAddress)
                    .ThenBy(r => r.FirstName)
                    .ThenBy(r => r.LastName),

            UserSort.NAME_DESC
                => query
                    .OrderByDescending(r => r.FirstName)
                    .ThenByDescending(r => r.LastName)
                    .ThenByDescending(r => r.EmailAddress),

            _ => query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName).ThenBy(r => r.EmailAddress)
        };

        if (request.Page > 1)
        {
            query = query.Skip((request.Page - 1) * request.Size);
        }

        query = query.Take(request.Size);

        var users = await query.ProjectToType<SearchUserResponse>().ToListAsync(cancellationToken);

        var pageCount = (count + request.Size - 1) / request.Size;

        return Results.Ok(
            new SearchUsersResponse()
            {
                Items = users,
                HasNextPage = request.Page < pageCount,
                HasPreviousPage = request.Page > 1
            }
        );
    }
}
