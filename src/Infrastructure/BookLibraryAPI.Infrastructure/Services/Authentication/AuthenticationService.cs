using BookLibraryAPI.Application.Common.Services.Authentication;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Core.Domain.Users;
using Microsoft.Extensions.Logging;

namespace BookLibraryAPI.Infrastructure.Services.Authentication;

public class AuthenticationService(
    IUserRepository userRepository,
    ILogger<AuthenticationService> logger)
    : IAuthenticationService
{
    public async Task<Result<User?>> ValidateUserAsync(
        string username,
        string password, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Validating user: {Username}", username);
            
            var user = await userRepository.GetByUsernameAsync(username, cancellationToken);
            
            if (user == null)
            {
                logger.LogWarning("User not found: {Username}", username);
                return Result<User?>.Success(null);
            }

            if (!VerifyPassword(password, user.PasswordHash).Value)
            {
                logger.LogWarning("Invalid password for user: {Username}", username);
                return Result<User?>.Success(null);
            }

            logger.LogDebug("User validated successfully: {Username}", username);
            return Result<User?>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating user: {Username}", username);
            return Result<User?>.Failure("Error validating user");
        }
    }

    public Result<string> HashPassword(string password)
    {
        return Result<string>.Success(BCrypt.Net.BCrypt.HashPassword(password));
    }

    public Result<bool> VerifyPassword(string password, string hash)
    {
        try
        {
            return Result<bool>.Success(BCrypt.Net.BCrypt.Verify(password, hash));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying password");
            return Result<bool>.Failure("Error verifying password");
        }
    }
}