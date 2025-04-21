using System.Reflection;
using FluentValidation;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.ForgotPassword;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;
using Halcyon.Api.Features.Notifications;
using Halcyon.Common.Authentication;
using Halcyon.Common.Cache;
using Halcyon.Common.Database;
using Halcyon.Common.Database.EntityChanged;
using Halcyon.Common.Database.Migration;
using Halcyon.Common.Email;
using Halcyon.Common.Infrastructure;
using Halcyon.Common.Messaging;
using Mapster;
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
            .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
            .Enrich.WithProperty("Version", version)
);

builder.AddDbContext<HalcyonDbContext>(connectionName: "Database");
builder.AddRabbitMq(connectionName: "RabbitMq", assembly);
builder.AddRedisDistributedCache(connectionName: "Redis");
builder.AddFluentEmail();

var seedConfig = builder.Configuration.GetSection(SeedSettings.SectionName);
builder.Services.Configure<SeedSettings>(seedConfig);
builder.Services.AddMigration<HalcyonDbContext, HalcyonDbSeeder>();

TypeAdapterConfig.GlobalSettings.Scan(assembly);
builder.Services.AddHybridCache();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.ConfigureJsonOptions();
builder.AddAuthentication();
builder.AddCors();
builder.AddSignalR();
builder.AddOpenTelemetry(version);
builder.AddOpenApi(version);

builder.AddSecurityServices();
builder.AddEntityChangedServices();

builder.Services.AddHostedService<ConsumerBackgroundService<ResetPasswordRequestedEvent>>();
builder.Services.AddHostedService<ConsumerBackgroundService<EntityChangedEvent>>();

builder.Services.AddScoped<IConsumer<ResetPasswordRequestedEvent>, SendResetPasswordEmailConsumer>();
builder.Services.AddScoped<IConsumer<EntityChangedEvent>, NotifyEntityChangedConsumer>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApiWithSwagger();
app.MapEndpoints(assembly);
app.MapHubs(assembly);
app.MapHealthChecks("/health");

app.Run();
