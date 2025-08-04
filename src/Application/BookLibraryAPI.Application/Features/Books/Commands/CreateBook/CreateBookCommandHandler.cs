using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Entities;
using BookLibraryAPI.Core.Domain.Exceptions;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookLibraryAPI.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create(request.Title, request.Author, request.Year);

        var createdBook = await bookRepository.AddAsync(book, cancellationToken);
    
        if (createdBook is null)
        {
            return Result<BookDto>.Failure(Error.NullValue.Name);
        }
       
        var dto = new BookDto(
            createdBook.Id,
            createdBook.Title.Value,
            createdBook.Author.Value,
            createdBook.Year,
            createdBook.CreatedAt);
        return Result<BookDto>.Success(dto);
    }
}