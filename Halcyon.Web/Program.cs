using Halcyon.Web.Data;
using Halcyon.Web.Filters;
using Halcyon.Web.Models;
using Halcyon.Web.Services.Config;
using Halcyon.Web.Services.Email;
using Halcyon.Web.Services.Hash;
using Halcyon.Web.Services.Jwt;
using Halcyon.Web.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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

builder.Configuration.Sources.Add(new AzureConfigurationSource());

var connectionString = builder.Configuration["ConnectionStrings:HalcyonDatabase"].Trim('"');

builder.Services.AddDbContext<HalcyonDbContext>((provider, options) =>
    options
        .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
        .UseNpgsql(
            connectionString,
            builder => builder.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = "role",
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]))
        };
    });

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilterAttribute));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(new ApiResponse<List<string>>
        {
            Code = "INVALID_REQUEST",
            Message = "Request is invalid.",
            Data = context.ModelState.Values
                .SelectMany(error => error.Errors)
                .Select(error => error.ErrorMessage)
                .ToList()
        });
    };
});

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
                Reference = new OpenApiReference
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
    .AddDbContextCheck<HalcyonDbContext>("database");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => policy
            .WithOrigins("http://localhost:3000", "https://*.chrispoulter.com")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithMethods("GET", "POST", "PUT", "OPTIONS")
            .WithHeaders(HeaderNames.Authorization));
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SeedSettings>(builder.Configuration.GetSection("Seed"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

app.UseHttpsRedirection();

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
app.Run();