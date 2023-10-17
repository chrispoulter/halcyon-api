using System.Reflection;

namespace Halcyon.Api.Features
{
    public interface IEndpoint
    {
        static abstract IEndpointRouteBuilder Map(IEndpointRouteBuilder endpoints);
    }

    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var mappers = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.GetInterfaces().Contains(typeof(IEndpoint)));

            foreach (var type in mappers)
            {
                type.GetMethod(nameof(IEndpoint.Map))
                    .Invoke(null, new object[] { endpoints });
            }

            return endpoints;
        }
    }
}
