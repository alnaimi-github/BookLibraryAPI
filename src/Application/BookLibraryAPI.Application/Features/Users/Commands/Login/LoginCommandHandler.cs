using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Application.Common.Mappers.Users;
using BookLibraryAPI.Application.Common.Services.Authentication;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Users.Commands.Login;

public class LoginCommandHandler(
    IAuthenticationService authenticationService,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, Result<AuthenticationResultDto>>
{
    public async Task<Result<AuthenticationResultDto>> Handle(
        LoginCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await authenticationService.ValidateUserAsync(request.Username, request.Password, cancellationToken);
            if (user is null)
            {
                return Result<AuthenticationResultDto>.Failure("Invalid username or password");
            }
            var tokenResult = tokenService.GenerateToken(user.Value!);
            return AuthenticationResultMapper.ToResultDto(tokenResult, user.Value.Role.ToString());
        }
        catch (Exception ex)
        {
            return Result<AuthenticationResultDto>.Failure("An error occurred during authentication");
        }
    }
}