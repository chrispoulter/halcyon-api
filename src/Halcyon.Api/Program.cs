using Carter;
using FluentValidation;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Seed;
using Halcyon.Api.Services.Date;
using Halcyon.Api.Services.Email;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Mapster;
using MassTransit;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var assembly = typeof(Program).Assembly;

var version = assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("HalcyonDatabase");

builder.Services.AddDbContext<HalcyonDbContext>((provider, options) =>
{
    options
        .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
        .UseNpgsql(dbConnectionString, builder => builder.EnableRetryOnFailure())
        .UseSnakeCaseNamingConvention();
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = JwtClaimNames.Roles,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserAdministratorPolicy", policy =>
          policy.RequireRole("SYSTEM_ADMINISTRATOR", "USER_ADMINISTRATOR"));
});

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => policy
            .WithOrigins("http://localhost:3000", "https://*.chrispoulter.com")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Options)
            .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType)
    );
});

builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.AddMassTransit(options =>
{
    options.AddConsumers(assembly);

    options.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<HalcyonDbContext>();

builder.Services.AddCarter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(version, new OpenApiInfo
    {
        Version = version,
        Title = "Halcyon API",
        Description = "A web api template."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Description = "Please insert JWT token into field"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

TypeAdapterConfig.GlobalSettings.Scan(assembly);

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ITemplateEngine, TemplateEngine>();

builder.Services.Configure<SeedSettings>(builder.Configuration.GetSection(SeedSettings.SectionName));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

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

app.MapCarter();
app.Run();