using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Application.Common.Mappers.Users;
using BookLibraryAPI.Application.Features.Users.Commands.Login;
using BookLibraryAPI.Application.Features.Users.Commands.Register;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryAPI.Presentation.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        auth.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Authenticate user and get JWT token")
            .WithDescription("Authenticate with username and password to receive a JWT token")
            .Produces<AuthenticationResultDto>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(500)
            .WithName("Login")
            .AllowAnonymous();

        auth.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Register a new user")
            .Produces<AuthenticationResultDto>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(500)
            .WithDescription("Register a new user with username, password, and role (Admin or User or Moderator)")
            .WithName("Register")
            .AllowAnonymous();
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginDto request,
        IMediator mediator,
        IValidator<LoginCommand> validator,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return Results.ValidationProblem(errors);
        }

        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            var errors = new Dictionary<string, string[]> { { "Authentication", new[] { result.Error ?? "Authentication failed" } } };
            return Results.ValidationProblem(errors, statusCode: 400);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> RegisterAsync(
        [FromBody] RegisterDto request,
        IMediator mediator,
        IValidator<RegisterCommand> validator,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return Results.ValidationProblem(errors);
        }

        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            var errors = new Dictionary<string, string[]> { { "Registration", new[] { result.Error ?? "Registration failed" } } };
            return Results.ValidationProblem(errors, statusCode: 400);
        }

        return Results.Ok(result.Value);
    }
}