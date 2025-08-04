using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Core.Domain.Common;

namespace BookLibraryAPI.Application.Common.Mappers.Users;

public static class AuthenticationResultMapper
{
    public static Result<AuthenticationResultDto> ToResultDto(bool isSuccess, string? token, string? role, string? errorMessage)
        => isSuccess
            ? Result<AuthenticationResultDto>.Success(new AuthenticationResultDto(true, token, role, null))
            : Result<AuthenticationResultDto>.Failure(errorMessage ?? "Authentication failed");

    public static AuthenticationResultDto ToDto(this Result<string> tokenResult, string? role)
        => tokenResult.IsSuccess
            ? new AuthenticationResultDto(true, tokenResult.Value, role, null)
            : new AuthenticationResultDto(false, null, null, tokenResult.Error);

    public static Result<AuthenticationResultDto> ToResultDto(Result<string> tokenResult, string role)
    {
        if (tokenResult.IsSuccess)
        {
            return Result<AuthenticationResultDto>.Success(
                new AuthenticationResultDto(true, tokenResult.Value, role, null)
            );
        }
        else
        {
            return Result<AuthenticationResultDto>.Failure(tokenResult.Error ?? "Token generation failed");
        }
    }
}
