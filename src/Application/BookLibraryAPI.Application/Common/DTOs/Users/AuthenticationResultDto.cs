namespace BookLibraryAPI.Application.Common.DTOs.Users;


public sealed record AuthenticationResultDto(
    bool IsSuccess,
    string? Token,
    string? Role, 
    string? ErrorMessage);