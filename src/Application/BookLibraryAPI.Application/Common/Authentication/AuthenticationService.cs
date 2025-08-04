using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Users;

namespace BookLibraryAPI.Application.Common.Authentication;

public interface IAuthenticationService
{
    Task<Result<User?>> ValidateUserAsync(string username, string password, CancellationToken cancellationToken = default);
    Result<string> HashPassword(string password);
    Result<bool> VerifyPassword(string password, string hash);
}