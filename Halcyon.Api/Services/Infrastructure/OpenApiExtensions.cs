﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace Halcyon.Api.Services.Infrastructure;

public static class OpenApiExtensions
{
    public static IHostApplicationBuilder AddOpenApi(
        this IHostApplicationBuilder builder,
        string version
    )
    {
        builder.Services.AddOpenApi(
            version,
            options =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Reference = new()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme,
                    },
                };

                options.AddDocumentTransformer(
                    (document, context, cancellationToken) =>
                    {
                        document.Components ??= new();
                        document.Components.SecuritySchemes.Add(
                            JwtBearerDefaults.AuthenticationScheme,
                            scheme
                        );

                        return Task.CompletedTask;
                    }
                );

                options.AddOperationTransformer(
                    (operation, context, cancellationToken) =>
                    {
                        if (
                            context
                                .Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>()
                                .Any()
                        )
                        {
                            operation.Security = [new() { [scheme] = [] }];
                        }

                        return Task.CompletedTask;
                    }
                );
            }
        );

        return builder;
    }

    public static WebApplication MapOpenApiWithSwagger(this WebApplication app, string version)
    {
        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/openapi/{version}.json", version);
            options.RoutePrefix = string.Empty;
        });

        return app;
    }
}
