using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Halcyon.Api.Core.Authentication;
using Halcyon.Api.Core.Database;
using Halcyon.Api.Core.Email;
using Halcyon.Api.Core.Web;
using Halcyon.Api.Data;
using Halcyon.Api.Features;
using Halcyon.Api.Features.Seed;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var assembly = typeof(Program).Assembly;

var version = assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Version", version)
    .CreateLogger();

builder.Host.UseSerilog();

var databaseConnectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<HalcyonDbContext>(
    (provider, options) =>
        options
            .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
            .UseNpgsql(databaseConnectionString, builder => builder.EnableRetryOnFailure())
            .UseSnakeCaseNamingConvention()
);

builder.Services.AddHostedService<MigrationHostedService<HalcyonDbContext>>();
builder.Services.AddHealthChecks().AddDbContextCheck<HalcyonDbContext>();

builder.Services.AddMassTransit(options =>
{
    options.AddConsumers(assembly);

    options.UsingInMemory(
        (context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
            cfg.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
        }
    );
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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
            ),
        }
    );

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy(
        nameof(AuthPolicy.IsUserAdministrator),
        policy => policy.RequireRole(AuthPolicy.IsUserAdministrator)
    );

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(
    version,
    options =>
    {
        options.AddDocumentTransformer(
            (document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Version = version,
                    Title = "Halcyon API",
                    Description =
                        "A .NET Core REST API project template. Built with a sense of peace and tranquillity.",
                };

                document.Servers.Clear();

                return Task.CompletedTask;
            }
        );

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

TypeAdapterConfig.GlobalSettings.Scan(assembly);
builder.Services.AddValidatorsFromAssembly(assembly);

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

//app.UseHttpsRedirection();
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

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/openapi/{version}.json", version);
    options.DocumentTitle = "Halcyon API";
    options.RoutePrefix = string.Empty;
});

app.MapEndpoints();
app.Run();

public partial class Program { }
