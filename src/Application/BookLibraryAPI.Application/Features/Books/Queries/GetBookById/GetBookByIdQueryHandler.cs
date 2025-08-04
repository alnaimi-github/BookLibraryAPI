using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Common.Mappers.Books;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler(IBookRepository bookRepository)
    : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            return Result<BookDto>.Failure($"Book with ID {request.Id} not found.");
        }
        return Result<BookDto>.Success(book.ToDto());
    }
}

