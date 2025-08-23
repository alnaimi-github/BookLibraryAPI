using BookLibraryAPI.Application;
using BookLibraryAPI.Infrastructure;
using BookLibraryAPI.Presentation.Endpoints;
using BookLibraryAPI.Presentation.Extensions;
using BookLibraryAPI.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerWithJwtAuth();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.ApplyMigrations();
app.Seed();


app.UseExceptionHandler();
app.UseCustomExceptionHandler();


app.UseAuthentication();
app.UseAuthorization();

app.MapBookEndpoints();
app.MapAuthenticationEndpoints();

app.Run();