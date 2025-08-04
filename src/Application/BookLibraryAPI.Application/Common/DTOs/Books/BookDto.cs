namespace BookLibraryAPI.Application.Common.DTOs.Books;

public sealed record BookDto(
    int Id, 
    string Title,
    string Author,
    int Year,
    DateTime CreatedAt);