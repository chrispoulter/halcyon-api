using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Halcyon.Api.Common.Database.EntityChanged;

public static class EntityChangedExtensions
{
    public static IHostApplicationBuilder AddEntityChangedServices(
        this IHostApplicationBuilder builder
    )
    {
        builder.Services.AddScoped<IInterceptor, EntityChangedInterceptor>();

        return builder;
    }
}
