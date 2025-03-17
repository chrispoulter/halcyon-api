using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("pgPassword", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", databaseName: "halcyon");

var rabbitMqPassword = builder.AddParameter("rmqPassword", secret: true);

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", password: rabbitMqPassword, port: 5672)
    .WithDataVolume(isReadOnly: false)
    .WithManagementPlugin(port: 15672)
    .WithLifetime(ContainerLifetime.Persistent);

var redis = builder
    .AddRedis("redis", port: 6379)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var maildevPassword = builder.AddParameter("mdPassword", secret: true);

var maildev = builder
    .AddMailDev("mail", password: maildevPassword, httpPort: 1080, smtpPort: 1025)
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

var jwtSecurityKey = builder.AddParameter("jwtSecurityKey", secret: true);
var jwtIssuer = builder.AddParameter("jwtIssuer");
var jwtAudience = builder.AddParameter("jwtAudience");

var api = builder
    .AddProject<Halcyon_Api>("api")
    .WithEnvironment("Jwt__SecurityKey", jwtSecurityKey)
    .WithEnvironment("Jwt__Issuer", jwtIssuer)
    .WithEnvironment("Jwt__Audience", jwtAudience)
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(maildev)
    .WaitFor(maildev);

var web = builder
    .AddNpmApp("web", "../Halcyon.Web", scriptName: "dev")
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("https"))
    .WithHttpEndpoint(port: 5173, env: "VITE_PORT", isProxied: false)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .WaitFor(api);

api.WithEnvironment("Email__CdnUrl", web.GetEndpoint("http"));

builder.Build().Run();
