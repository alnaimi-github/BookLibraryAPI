using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Application.Common.Mappers.Users;
using BookLibraryAPI.Application.Features.Users.Commands.Login;
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
            .Produces(500);
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
            throw new ValidationException(validationResult.Errors);
        }

        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            var errors = new Dictionary<string, string[]> { 
            { "Authentication", [result.Error ?? "Authentication failed"] } };
            
            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.Ok(result);
    }
}