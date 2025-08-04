using BookLibraryAPI.Application;
using BookLibraryAPI.Infrastructure;
using BookLibraryAPI.Infrastructure.Persistence;
using BookLibraryAPI.Presentation.Extensions;
using BookLibraryAPI.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.ApplyMigrations();
    
    app.Seed();
}

app.UseCustomExceptionHandler();

app.Run();