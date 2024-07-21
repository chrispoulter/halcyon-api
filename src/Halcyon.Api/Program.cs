using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FluentValidation;
using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features;
using Halcyon.Api.Features.Messaging;
using Halcyon.Api.Features.Seed;
using Halcyon.Api.Services.Email;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Halcyon.Api.Services.Migrations;
using Mapster;
using MassTransit;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var assembly = typeof(Program).Assembly;

var version = assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;

var builder = WebApplication.CreateBuilder(args);

var tenant = builder.Configuration["Tenant"];

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

var databaseConnectionString = builder.Configuration.GetConnectionString("Database");

builder
    .Services.AddDbContext<HalcyonDbContext>(
        (provider, options) =>
        {
            options
                .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
                .UseNpgsql(databaseConnectionString, builder => builder.EnableRetryOnFailure())
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(provider.GetRequiredService<EntityChangedInterceptor>());
        }
    )
    .AddHostedService<MigrationHostedService<HalcyonDbContext>>();

builder.Services.AddMassTransit(options =>
{
    var namePrefix = $"{ApplicationNameRegex().Replace(tenant, "-")}-";

    options.AddConsumers(assembly);
    options.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(namePrefix));

    var serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBus");

    if (!string.IsNullOrEmpty(serviceBusConnectionString))
    {
        options.AddConfigureEndpointsCallback(
            (_, cfg) =>
            {
                if (cfg is IServiceBusReceiveEndpointConfigurator sb)
                {
                    sb.ConfigureDeadLetterQueueDeadLetterTransport();
                    sb.ConfigureDeadLetterQueueErrorTransport();
                }
            }
        );

        options.UsingAzureServiceBus(
            (context, cfg) =>
            {
                cfg.Host(serviceBusConnectionString);
                cfg.ConfigureEndpoints(context);
                cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
                cfg.MessageTopology.SetEntityNameFormatter(
                    new PrefixEntityNameFormatter(
                        cfg.MessageTopology.EntityNameFormatter,
                        namePrefix
                    )
                );
            }
        );
    }
    else
    {
        options.UsingInMemory(
            (context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
                cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
            }
        );
    }
});

var signalRBuilder = builder
    .Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var signalRConnectionString = builder.Configuration.GetConnectionString("SignalR");

if (!string.IsNullOrEmpty(signalRConnectionString))
{
    var applicationName = ApplicationNameRegex().Replace(tenant, "_");

    signalRBuilder.AddAzureSignalR(configure =>
    {
        configure.ConnectionString = signalRConnectionString;
        configure.ApplicationName = applicationName;
    });
}

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messages"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy(
        nameof(Policy.IsUserAdministrator),
        policy => policy.RequireRole(Policy.IsUserAdministrator)
    );

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var corsPolicySettings = new CorsPolicySettings();
builder.Configuration.GetSection(CorsPolicySettings.SectionName).Bind(corsPolicySettings);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithOrigins(corsPolicySettings.AllowedOrigins)
            .WithMethods(corsPolicySettings.AllowedMethods)
            .WithHeaders(corsPolicySettings.AllowedHeaders)
            .AllowCredentials()
    )
);

builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddHealthChecks().AddDbContextCheck<HalcyonDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        version,
        new OpenApiInfo
        {
            Version = version,
            Title = "Halcyon API",
            Description = "A web api template."
        }
    );

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Scheme = "bearer",
            Description = "Please insert JWT token into field"
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        }
    );
});

TypeAdapterConfig.GlobalSettings.Scan(assembly);

builder.Services.AddScoped<EntityChangedInterceptor>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName)
);
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ITemplateEngine, TemplateEngine>();

builder.Services.Configure<SeedSettings>(
    builder.Configuration.GetSection(SeedSettings.SectionName)
);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
    options.DocumentTitle = "Halcyon API";
    options.RoutePrefix = string.Empty;
});

app.MapHub<MessageHub>("/messages");
app.MapEndpoints();
app.Run();

public partial class Program
{
    [GeneratedRegex("[^A-Za-z0-9]")]
    private static partial Regex ApplicationNameRegex();
}
