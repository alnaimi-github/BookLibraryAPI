namespace BookLibraryAPI.Application.Common.DTOs.Books;

public sealed record CreateBookDto(string Title, string Author, int Year);