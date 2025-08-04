namespace BookLibraryAPI.Application.Common.DTOs.Books;

public sealed record UpdateBookDto(int Id, string Title, string Author, int Year);