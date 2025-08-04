using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Common.Mappers.Books;
using BookLibraryAPI.Application.Features.Books.Commands.CreateBook;
using BookLibraryAPI.Application.Features.Books.Queries.GetAllBooks;
using BookLibraryAPI.Application.Features.Books.Queries.GetBookById;
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
            .Produces(500)
            .WithName("CreateBook");

        books.MapGet("/", GetAllBooksAsync)
            .WithName("GetAllBooks")
            .WithSummary("Get all books")
            .WithDescription("Retrieve all books from the library (requires authentication)")
            .Produces<IEnumerable<BookDto>>(200)
            .Produces(401)
            .Produces(500)
            .WithName("GetAllBooks")
            .RequireAuthorization();

        books.MapGet("/{id:int}", GetBookByIdAsync)
            .WithName("GetBookById")
            .WithSummary("Get a book by ID")
            .WithDescription("Retrieve a single book by its ID.")
            .Produces<BookDto>(200)
            .Produces(404)
            .Produces(500)
            .WithName("GetBookById")
            .RequireAuthorization();
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
        return Results.CreatedAtRoute("GetBookById", new { id = result.Value.Id }, result.Value);
    }
    
    private static async Task<IResult> GetAllBooksAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllBooksQuery(), cancellationToken);
        
        if ( !result.IsSuccess || result.Value == null || !result.Value.Any())
        {
            var problemDetails = new ProblemDetails
            {
                Title = "No books found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            };
            return Results.Problem(
                problemDetails.Detail, 
                statusCode: problemDetails.Status, 
                title: problemDetails.Title);
        }
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetBookByIdAsync(
        int id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBookByIdQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Value == null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Book not found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            };
            return Results.Problem(
                problemDetails.Detail,
                statusCode: problemDetails.Status,
                title: problemDetails.Title);
        }
        return Results.Ok(result.Value);
    }
}