using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Users;

namespace BookLibraryAPI.Application.Common.Services.Authentication;

public interface ITokenService
{
    Result<string> GenerateToken(User user);
    Result<bool> ValidateToken(string token);
}