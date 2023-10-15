using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.ForgotPassword;
using Halcyon.Api.Features.Account.Register;
using Halcyon.Api.Features.Account.ResetPassword;
using Halcyon.Api.Features.Seed;
using Halcyon.Api.Features.Token.CreateToken;
using Halcyon.Api.Services.Date;
using Halcyon.Api.Services.Email;
using Halcyon.Api.Services.Hash;
using Halcyon.Api.Services.Jwt;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var version = Assembly.GetEntryAssembly()
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    .InformationalVersion;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HalcyonDatabase");

builder.Services.AddDbContext<HalcyonDbContext>((provider, options) =>
{
    options
        .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
        .UseNpgsql(connectionString, builder => builder.EnableRetryOnFailure())
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddMassTransit(options =>
{
    options.AddConsumers(Assembly.GetExecutingAssembly());

    options.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

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

builder.Services.AddHealthChecks()
    .AddDbContextCheck<HalcyonDbContext>();

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

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<SeedSettings>(builder.Configuration.GetSection(SeedSettings.SectionName));

builder.Services.AddSingleton<IDateService, DateService>();
builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<IJwtService, JwtService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler("/error");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", version);
    options.DocumentTitle = "Halcyon API";
    options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.UseCors();
app.MapControllers();

app.MapRegisterEndpoint()
    .MapForgotPasswordEndpoint()
    .MapResetPasswordEndpoint();

app.MapSeedEndpoint();

app.MapCreateTokenEndpoint();

app.Run();