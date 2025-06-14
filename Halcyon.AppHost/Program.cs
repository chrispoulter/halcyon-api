var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent)
    .PublishAsConnectionString();

var database = postgres.AddDatabase("database", databaseName: "halcyon-dotnet");

var maildev = builder
    .AddMailDev("mail", httpPort: 1080, smtpPort: 1025)
    .WithLifetime(ContainerLifetime.Persistent)
    .PublishAsConnectionString();

var api = builder
    .AddProject<Projects.Halcyon_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database)
    .WithReference(maildev)
    .WaitFor(maildev);

var web = builder
    .AddNpmApp("web", "../Halcyon.Web", scriptName: "dev")
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT", port: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .WithReference(api)
    .WaitFor(api);

api.WithEnvironment("Email__SiteUrl", web.GetEndpoint("http"));

builder.Build().Run();
