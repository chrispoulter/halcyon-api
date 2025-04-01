using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Halcyon.Common.Infrastructure;

public static class CorsExtensions
{
    public static IHostApplicationBuilder AddCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        return builder;
    }
}
