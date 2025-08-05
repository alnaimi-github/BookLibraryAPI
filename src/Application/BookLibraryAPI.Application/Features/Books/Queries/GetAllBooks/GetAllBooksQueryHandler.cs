using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Common.Mappers.Books;
using BookLibraryAPI.Application.Common.Services.Caching;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Queries.GetAllBooks;

public class GetAllBooksQueryHandler(
    IBookRepository bookRepository,
    ICacheService cacheService)
    : IRequestHandler<GetAllBooksQuery, Result<IEnumerable<BookDto>>>
{
    public async Task<Result<IEnumerable<BookDto>>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        const string cacheKey = "books:all";
        var cached = await cacheService.GetAsync<IEnumerable<BookDto>>(cacheKey, cancellationToken);
        
        if (cached is not null)
        {
            return Result<IEnumerable<BookDto>>.Success(cached);
        }

        var books = await bookRepository.GetAllAsync(cancellationToken);
        if (books is null || !books.Any())
        {
            return Result<IEnumerable<BookDto>>.Failure("No books found.");
        }
        var dtos = books.ToDtoList();
        
        await cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);
        
        return Result<IEnumerable<BookDto>>.Success(dtos);
    }
}