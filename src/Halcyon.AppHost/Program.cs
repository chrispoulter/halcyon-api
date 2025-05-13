using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("pgPassword", secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword, port: 5432)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("database", databaseName: "halcyon-api");

var rabbitMqPassword = builder.AddParameter("rmqPassword", secret: true);

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", password: rabbitMqPassword, port: 5672)
    .WithDataVolume(isReadOnly: false)
    .WithManagementPlugin(port: 15672)
    .WithLifetime(ContainerLifetime.Persistent);

var redisPassword = builder.AddParameter("rdPassword", secret: true);

var redis = builder
    .AddRedis("redis", password: redisPassword, port: 6379)
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

var maildevPassword = builder.AddParameter("mdPassword", secret: true);

var maildev = builder
    .AddMailDev("mail", password: maildevPassword, httpPort: 1080, smtpPort: 1025)
    .WithExternalHttpEndpoints()
    .WithLifetime(ContainerLifetime.Persistent);

builder
    .AddProject<Halcyon_Api>("api")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(maildev)
    .WaitFor(maildev);

builder.Build().Run();
