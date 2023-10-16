using System.Reflection;

namespace Halcyon.Api.Features
{
    public interface IEndpoint
    {
        WebApplication MapEndpoint(WebApplication app);
    }

    public static class EndpointExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            var endpoints = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.GetInterfaces().Contains(typeof(IEndpoint)));

            foreach (var type in endpoints)
            {
                var item = (IEndpoint)Activator.CreateInstance(type);
                item.MapEndpoint(app);
            }

            return app;
        }
    }
}
