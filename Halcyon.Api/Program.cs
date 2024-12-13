using System.Reflection;
using FluentValidation;
using Halcyon.Api.Data;
using Halcyon.Api.Services.Authentication;
using Halcyon.Api.Services.Database;
using Halcyon.Api.Services.Email;
using Halcyon.Api.Services.Events;
using Halcyon.Api.Services.Infrastructure;
using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

var assembly = typeof(Program).Assembly;

var version = assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, loggerConfig) =>
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithProperty("Version", version)
);

builder.Services.AddDbContext<HalcyonDbContext>(
    (provider, options) =>
        options
            .UseNpgsql(
                builder.Configuration.GetConnectionString("Database"),
                builder => builder.EnableRetryOnFailure()
            )
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(provider.GetServices<IInterceptor>())
);

builder.Services.AddHealthChecks().AddDbContextCheck<HalcyonDbContext>();

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

builder.AddMassTransit(connectionName: "RabbitMq", assembly);
builder.AddRedis(connectionName: "Redis");

#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

TypeAdapterConfig.GlobalSettings.Scan(assembly);
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();

builder.ConfigureJsonOptions();
builder.AddAuthentication();
builder.AddCors();
builder.AddSignalR();
builder.AddOpenApi(version);
builder.AddAuthenticationServices();
builder.AddEmailServices();
builder.AddEventServices();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiWithSwagger(version);
app.MapEndpoints(assembly);
app.MapHubs(assembly);
app.MapHealthChecks("/health");

app.Run();
