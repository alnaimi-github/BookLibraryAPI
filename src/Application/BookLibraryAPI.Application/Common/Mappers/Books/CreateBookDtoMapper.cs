using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Features.Books.Commands.CreateBook;
using BookLibraryAPI.Core.Domain.Entities;

namespace BookLibraryAPI.Application.Common.Mappers.Books;

public static class CreateBookDtoMapper
{
    public static CreateBookCommand ToCommand(this CreateBookDto dto)
        => new(dto.Title, dto.Author, dto.Year);
    
    
    public static BookDto ToDto(this Book book) =>
        new(
            book.Id,
            book.Title.Value,
            book.Author.Value,
            book.Year,
            book.CreatedAt
        );
}

