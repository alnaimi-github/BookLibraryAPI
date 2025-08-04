using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Users.Enums;

namespace BookLibraryAPI.Core.Domain.Users;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    
    private User() { } // EF Core requires a parameterless constructor
    
    public static User Create(string username, string passwordHash, UserRole role)
    {
        return new User
        {
            Username = username.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role
        };
    }
    
}