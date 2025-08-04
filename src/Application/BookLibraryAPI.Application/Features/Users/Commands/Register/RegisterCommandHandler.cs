using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Application.Common.Mappers.Users;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Core.Domain.Users;
using MediatR;
using BookLibraryAPI.Application.Common.Services.Authentication;
using BookLibraryAPI.Core.Domain.Users.Enums;

namespace BookLibraryAPI.Application.Features.Users.Commands.Register;

public class RegisterCommandHandler(IUserRepository userRepository,IAuthenticationService authenticationService, ITokenService tokenService)
    : IRequestHandler<RegisterCommand, Result<AuthenticationResultDto>>
{
    public async Task<Result<AuthenticationResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await CheckUserExists(request.Username, cancellationToken);
        if (!userExistsResult.IsSuccess)
            return userExistsResult;

        var hashedPasswordResult = HashPassword(request.Password);
        if (!hashedPasswordResult.IsSuccess)
            return Result<AuthenticationResultDto>.Failure(hashedPasswordResult.Error ?? string.Empty);

        var roleResult = ParseRole(request.Role);
        if (!roleResult.IsSuccess)
            return Result<AuthenticationResultDto>.Failure(roleResult.Error ?? string.Empty);

        var user = User.Create(request.Username, hashedPasswordResult.Value ?? string.Empty, roleResult.Value);
        await userRepository.AddAsync(user, cancellationToken);

        var tokenResult = tokenService.GenerateToken(user);
        return AuthenticationResultMapper.ToResultDto(tokenResult, user.Role.ToString());
    }

    private async Task<Result<AuthenticationResultDto>> CheckUserExists(string username, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByUsernameAsync(username, cancellationToken);
        return existingUser == null
            ? Result<AuthenticationResultDto>.Success(null)
            : Result<AuthenticationResultDto>.Failure("Username already exists");
    }

    private Result<string> HashPassword(string password) =>
        authenticationService.HashPassword(password);

    private Result<UserRole> ParseRole(string role) =>
        Enum.TryParse<UserRole>(role, true, out var userRole)
            ? Result<UserRole>.Success(userRole)
            : Result<UserRole>.Failure("Invalid role specified");
}
