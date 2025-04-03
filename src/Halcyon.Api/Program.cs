using FluentValidation;
using Halcyon.Api.Data;
using Halcyon.Common.Authentication;
using Halcyon.Common.Database;
using Halcyon.Common.Email;
using Halcyon.Common.Events;
using Halcyon.Common.Infrastructure;
using Mapster;
using MassTransit;

var assembly = typeof(Program).Assembly;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddDbContext<HalcyonDbContext>(connectionName: "Database");
builder.AddMassTransit(connectionName: "RabbitMq", assembly);
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddFluentEmail(connectionName: "Mail");

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

TypeAdapterConfig.GlobalSettings.Scan(assembly);
builder.Services.AddHybridCache();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();

builder.ConfigureJsonOptions();
builder.AddAuthentication();
builder.AddCors();
builder.AddSignalR();
builder.AddOpenApi();
builder.AddAuthenticationServices();
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
