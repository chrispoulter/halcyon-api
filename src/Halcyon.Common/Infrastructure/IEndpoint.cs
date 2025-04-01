using Microsoft.AspNetCore.Routing;

namespace Halcyon.Common.Infrastructure;

public interface IEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
