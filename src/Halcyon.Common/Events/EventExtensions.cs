using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Events;

public static class EventExtensions
{
    public static IHostApplicationBuilder AddEventServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IInterceptor, EntityChangedInterceptor>();

        return builder;
    }
}
