using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.SearchUsers;

public class SearchUsersEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/user", HandleAsync)
            .RequireAuthorization(nameof(AuthPolicy.IsUserAdministrator))
            .AddValidationFilter<SearchUsersRequest>()
            .WithTags(EndpointTag.Users)
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
            query = query.Where(e =>
                EF.Functions.Like(
                    e.FirstName + " " + e.LastName + " " + e.EmailAddress,
                    $"%{request.Search}%"
                )
            );
        }

        var count = await query.CountAsync(cancellationToken);

        query = request.Sort switch
        {
            UserSort.EMAIL_ADDRESS_DESC => query
                .OrderByDescending(r => r.EmailAddress)
                .ThenBy(r => r.Id),

            UserSort.EMAIL_ADDRESS_ASC => query.OrderBy(r => r.EmailAddress).ThenBy(r => r.Id),

            UserSort.NAME_DESC => query
                .OrderByDescending(r => r.FirstName)
                .ThenByDescending(r => r.LastName)
                .ThenBy(r => r.Id),

            _ => query.OrderBy(r => r.FirstName).ThenBy(r => r.LastName).ThenBy(r => r.Id),
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
                HasPreviousPage = request.Page > 1 && request.Page <= pageCount,
            }
        );
    }
}
