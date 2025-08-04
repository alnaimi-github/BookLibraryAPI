using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Core.Domain.Entities;
using BookLibraryAPI.Core.Domain.Exceptions;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookLibraryAPI.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository)
    : IRequestHandler<CreateBookCommand, BookDto>
{

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create(request.Title, request.Author, request.Year);

        var createdBook = await bookRepository.AddAsync(book, cancellationToken);
        return new BookDto(
            createdBook.Id,
            createdBook.Title.Value,
            createdBook.Author.Value,
            createdBook.Year,
            createdBook.CreatedAt);
    }
}