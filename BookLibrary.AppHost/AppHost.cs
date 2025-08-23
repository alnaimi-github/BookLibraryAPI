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
    .WithEnvironment("ConnectionStrings__mailpit", email.GetEndpoint("smtp"))
    .WaitFor(bookLabDb)
    .WaitFor(email);

builder.Build().Run();
