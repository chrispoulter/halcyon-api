var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", databaseName: "halcyon-api");

var maildevPassword = builder.AddParameter("MailDevPassword", secret: true);

var maildev = builder
    .AddMailDev("mail", password: maildevPassword, httpPort: 1080, smtpPort: 1025)
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder
    .AddProject<Projects.Halcyon_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database)
    .WithReference(maildev)
    .WaitFor(maildev);

builder
    .AddNpmApp("web", "../Halcyon.Web", scriptName: "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT", port: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
