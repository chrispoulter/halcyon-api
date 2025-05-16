using System.Reflection;
using FluentValidation;
using Halcyon.Api.Common.Authentication;
using Halcyon.Api.Common.Database;
using Halcyon.Api.Common.Email;
using Halcyon.Api.Common.Infrastructure;
using Halcyon.Api.Data;

var assembly = typeof(Program).Assembly;

var version = assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;

var builder = WebApplication.CreateBuilder(args);

builder.AddDbContext<HalcyonDbContext>(connectionName: "Database");
builder.AddFluentEmail();

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.ConfigureJsonOptions();
builder.AddAuthentication();
builder.AddSecurityServices();
builder.AddCors();
builder.AddOpenTelemetry(version);
builder.AddOpenApi(version);

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiWithSwagger();
app.MapEndpoints(assembly);
app.MapHealthChecks("/health");

app.Run();
