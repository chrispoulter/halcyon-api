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

builder.AddDbContext<HalcyonDbContext>(connectionName: "Database");
builder.AddMassTransit(connectionName: "RabbitMq", assembly);
builder.AddRedisDistributedCache(connectionName: "Redis");

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018

TypeAdapterConfig.GlobalSettings.Scan(assembly);
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.ConfigureJsonOptions();
builder.AddAuthentication();
builder.AddCors();
builder.AddSignalR();
builder.AddOpenTelemetry(version);
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
