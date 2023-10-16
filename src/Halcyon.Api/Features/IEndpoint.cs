namespace Halcyon.Api.Features
{ 
    public interface IEndpoint
    {
        WebApplication MapEndpoint(WebApplication app);
    }
}