namespace BookLibraryAPI.Application.Common.DTOs.Users;

public sealed record UserDto(
    int Id, 
    string Username,
    string Role, 
    DateTime CreatedAt);