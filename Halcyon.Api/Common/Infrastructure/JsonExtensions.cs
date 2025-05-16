using System.Text.Json.Serialization;

namespace Halcyon.Api.Common.Infrastructure;

public static class JsonExtensions
{
    public static IHostApplicationBuilder ConfigureJsonOptions(this IHostApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return builder;
    }
}
