using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string Author,
    int Year) : IRequest<Result<BookDto>>;