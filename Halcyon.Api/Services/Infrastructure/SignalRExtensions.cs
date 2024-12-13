using System.Reflection;
using System.Text.Json.Serialization;
using Halcyon.Api.Services.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Services.Infrastructure;

public static class SignalRExtensions
{
    private static readonly MethodInfo _mapHubMethod = typeof(HubEndpointRouteBuilderExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .First(m => m.Name == "MapHub" && m.IsGenericMethodDefinition);

    public static IHostApplicationBuilder AddSignalR(this IHostApplicationBuilder builder)
    {
        builder
            .Services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return builder;
    }

    public static WebApplication MapHubs(this WebApplication app, Assembly assembly)
    {
        var hubs = assembly.DefinedTypes.Where(type =>
            type is { IsAbstract: false, IsInterface: false } && typeof(Hub).IsAssignableFrom(type)
        );

        foreach (var hub in hubs)
        {
            var genericMethod = _mapHubMethod.MakeGenericMethod(hub);
            var hubRoute = $"/hubs/{hub.Name}";
            genericMethod.Invoke(null, [app, hubRoute]);
        }

        return app;
    }
}
