namespace Halcyon.Api.Common.Infrastructure;

public interface IEndpoint
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
