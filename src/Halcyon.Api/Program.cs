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

var assembly = typeof(Program).Assembly;

var builder = WebApplication.CreateBuilder(args);

var databaseConnectionString = builder.Configuration.GetConnectionString("Database");

builder
    .Services.AddDbContext<HalcyonDbContext>(
        (provider, options) =>
            options
                .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
                .UseNpgsql(databaseConnectionString, builder => builder.EnableRetryOnFailure())
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(provider.GetServices<IInterceptor>())
    )
    .AddHealthChecks()
    .AddDbContextCheck<HalcyonDbContext>();

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

builder.AddMassTransit(connectionName: "RabbitMq", assembly);

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "HalcyonApi";
});

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
builder.AddOpenApi();
builder.AddAuthenticationServices();
builder.AddEmailServices();
builder.AddEventServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiWithSwagger();
app.MapEndpoints(assembly);
app.MapHubs(assembly);

app.Run();

public partial class Program { }
