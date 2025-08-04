using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Core.Domain.Users;
using BookLibraryAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibraryAPI.Infrastructure.Repositories.Users;

public class UserRepository(LibraryDbContext context) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()   
            .FirstOrDefaultAsync(u => u.Username == username.ToLowerInvariant(), 
                cancellationToken);
    }

    public async Task<User?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        return await context.Users.FindAsync([id], cancellationToken);
    }

    public async Task<User> AddAsync(User user, 
        CancellationToken cancellationToken = default)
    {

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User> UpdateAsync(User user, 
        CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, 
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == username.ToLowerInvariant(), cancellationToken);
    }
}