using Halcyon.Api.Common.Authorization;
using Halcyon.Api.Common.Infrastructure;
using Halcyon.Api.Common.Validation;
using Halcyon.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.SearchUsers;

public class SearchUsersEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/user", HandleAsync)
            .RequireRole(Roles.SystemAdministrator, Roles.UserAdministrator)
            .AddValidationFilter<SearchUsersRequest>()
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
            query = query.Where(u =>
                u.SearchVector.Matches(EF.Functions.PhraseToTsQuery("english", request.Search))
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

        var page = request.Page ?? 1;
        var size = request.Size ?? 10;

        if (page > 1)
        {
            query = query.Skip((page - 1) * size);
        }

        query = query.Take(size);

        var users = await query.ProjectToType<SearchUserResponse>().ToListAsync(cancellationToken);

        var pageCount = (count + size - 1) / size;

        return Results.Ok(
            new SearchUsersResponse()
            {
                Items = users,
                HasNextPage = page < pageCount,
                HasPreviousPage = page > 1 && page <= pageCount,
            }
        );
    }
}
