using BookLibraryAPI.Application.Common.DTOs.Books;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string Title,
    string Author,
    int Year) : IRequest<BookDto>;