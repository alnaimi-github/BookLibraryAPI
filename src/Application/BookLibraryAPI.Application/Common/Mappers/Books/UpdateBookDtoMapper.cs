using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Features.Books.Commands.UpdateBook;

namespace BookLibraryAPI.Application.Common.Mappers.Books;

public static class UpdateBookDtoMapper
{
    public static UpdateBookCommand ToCommand(this UpdateBookDto dto)
        => new(dto.Id, dto.Title, dto.Author, dto.Year);
}

