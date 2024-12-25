using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Halcyon.Api.Common.Events;

public static class EventExtensions
{
    public static IHostApplicationBuilder AddEventServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IInterceptor, EntityChangedInterceptor>();

        return builder;
    }
}
