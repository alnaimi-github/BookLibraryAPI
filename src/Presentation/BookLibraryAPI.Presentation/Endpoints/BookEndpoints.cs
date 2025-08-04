using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Common.Mappers.Books;
using BookLibraryAPI.Application.Features.Books.Commands.CreateBook;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryAPI.Presentation.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var books = endpoints.MapGroup("/api/books")
            .WithTags("Books")
            .WithOpenApi();

        books.MapPost("/", CreateBookAsync)
            .WithName("CreateBook")
            .WithSummary("Create a new book")
            .WithDescription("Add a new book to the library (requires Moderator or Admin role)")
            .Produces<BookDto>(201)
            .Produces<ValidationProblemDetails>(400)
            .Produces(401)
            .Produces(403)
            .Produces(500);
    }

    private static async Task<IResult> CreateBookAsync(
        [FromBody] CreateBookDto request,
        IMediator mediator,
        IValidator<CreateBookCommand> validator,
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
        if (!result.IsSuccess || result.Value == null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Book creation failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            };
            return Results.Problem(
                problemDetails.Detail,
                statusCode: problemDetails.Status, title: problemDetails.Title);
        }
        return Results.CreatedAtRoute($"/api/books/{result.Value.Id}", result.Value);
    }
}