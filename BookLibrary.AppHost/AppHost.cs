var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BookLibraryAPI_Presentation>("booklibraryapi-presentation");

builder.Build().Run();
