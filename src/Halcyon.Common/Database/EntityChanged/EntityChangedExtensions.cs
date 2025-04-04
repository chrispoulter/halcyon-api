using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Database.EntityChanged;

public static class EntityChangedExtensions
{
    public static IHostApplicationBuilder AddEntityChangedServices(
        this IHostApplicationBuilder builder
    )
    {
        builder.Services.AddTransient<IInterceptor, EntityChangedInterceptor>();

        return builder;
    }
}
