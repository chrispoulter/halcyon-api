using FluentValidation;
using Halcyon.Api.Common.Authentication;
using Halcyon.Api.Common.Database;
using Halcyon.Api.Common.Email;
using Halcyon.Api.Common.Events;
using Halcyon.Api.Common.Infrastructure;
using Halcyon.Api.Data;
using Mapster;
using MassTransit;

var assembly = typeof(Program).Assembly;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddDbContext<HalcyonDbContext>(connectionName: "Database");
builder.AddMassTransit(connectionName: "RabbitMq", assembly);
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddMailKitClient(connectionName: "Mail");

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

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
app.MapDefaultEndpoints();

app.Run();
