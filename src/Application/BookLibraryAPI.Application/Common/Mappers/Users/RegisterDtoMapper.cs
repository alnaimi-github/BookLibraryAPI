using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Application.Features.Users.Commands.Register;

namespace BookLibraryAPI.Application.Common.Mappers.Users;

public static class RegisterDtoMapper
{
    public static RegisterCommand ToCommand(this RegisterDto dto)
        => new(dto.Username, dto.Password, dto.Role);
}

