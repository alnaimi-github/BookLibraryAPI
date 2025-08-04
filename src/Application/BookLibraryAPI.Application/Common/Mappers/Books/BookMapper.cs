using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Features.Books.Commands.CreateBook;
using BookLibraryAPI.Core.Domain.Books;

namespace BookLibraryAPI.Application.Common.Mappers.Books;

public static class BookMapper
{
    public static BookDto ToDto(this Book book) =>
        new(
            book.Id,
            book.Title.Value,
            book.Author.Value,
            book.Year,
            book.CreatedAt
        );

    public static List<BookDto> ToDtoList(this IEnumerable<Book> books) =>
        books.Select(b => b.ToDto()).ToList();
    
    public static CreateBookCommand ToCommand(this CreateBookDto dto)
        => new(dto.Title, dto.Author, dto.Year);
}

