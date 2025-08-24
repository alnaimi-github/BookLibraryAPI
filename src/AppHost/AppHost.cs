var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    //.WithPgAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var bookLabDb = postgres.AddDatabase("bookLabDb");

var cache = builder
    .AddRedis("cache")
    .WithRedisInsight()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var email = builder
    .AddMailPit("mailpit");


builder.AddProject<Projects.BookLibraryAPI_Presentation>("booklibraryapi-presentation")
    .WithReference(bookLabDb)
    .WithReference(cache)
    .WithReference(email)
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint()
    .WithHttpsEndpoint()
    .WithEnvironment("EMAIL__FROM", "library@example.com")
    .WithEnvironment("EMAIL__TO", "admin@example.com")
    .WithEnvironment("MAILPIT__SMTP", email.GetEndpoint("smtp"))
    .WaitFor(bookLabDb)
    .WaitFor(email);

builder.Build().Run();
