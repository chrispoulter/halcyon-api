﻿using Halcyon.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Halcyon.Api.Services.Infrastructure;

public static class EntityFrameworkExtensions
{
    public static IHostApplicationBuilder AddDbContext<TContext>(
        this IHostApplicationBuilder builder,
        string connectionName
    )
        where TContext : DbContext
    {
        builder.Services.AddDbContext<HalcyonDbContext>(
            (provider, options) =>
                options
                    .UseSqlServer(
                        builder.Configuration.GetConnectionString(connectionName),
                        builder => builder.EnableRetryOnFailure()
                    )
                    .AddInterceptors(provider.GetServices<IInterceptor>())
        );

        builder.Services.AddHealthChecks().AddDbContextCheck<TContext>();

        return builder;
    }
}
