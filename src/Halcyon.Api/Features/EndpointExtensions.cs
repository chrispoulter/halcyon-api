using System.Reflection;

namespace Halcyon.Api.Features
{
    public interface IEndpoint
    {
        static abstract IEndpointRouteBuilder Map(IEndpointRouteBuilder builder);
    }

    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            var endpoints = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.GetInterfaces().Contains(typeof(IEndpoint)));

            foreach (var type in endpoints)
            {
                type.GetMethod(nameof(IEndpoint.Map))
                    .Invoke(null, new object[] { builder });
            }

            return builder;
        }
    }
}
