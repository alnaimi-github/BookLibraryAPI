using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Application.Features.Users.Commands.Login;

namespace BookLibraryAPI.Application.Common.Mappers.Users;

public static class LoginDtoMapper
{
    public static LoginCommand ToCommand(this LoginDto dto)
        => new(dto.Username, dto.Password);
}

