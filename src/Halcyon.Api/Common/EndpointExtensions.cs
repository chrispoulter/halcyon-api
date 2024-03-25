using System.Reflection;

namespace Halcyon.Api.Common;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var interfaceType = typeof(IEndpoint);

        var types = assembly
            .GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsInterface);

        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is IEndpoint instance)
            {
                instance.MapEndpoints(app);
            }
        }

        return app;
    }
}
