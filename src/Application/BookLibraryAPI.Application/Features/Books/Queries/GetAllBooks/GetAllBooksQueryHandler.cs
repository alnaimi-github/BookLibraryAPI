using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Common.Mappers.Books;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Queries.GetAllBooks;

public class GetAllBooksQueryHandler(IBookRepository bookRepository)
    : IRequestHandler<GetAllBooksQuery, Result<IEnumerable<BookDto>>>
{
    public async Task<Result<IEnumerable<BookDto>>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        var books = await bookRepository.GetAllAsync(cancellationToken);
        if (books is null || !books.Any())
        {
            return Result<IEnumerable<BookDto>>.Failure("No books found.");
        }
        var dtos = books.ToDtoList();
        return Result<IEnumerable<BookDto>>.Success(dtos);
    }
}