using BookLibraryAPI.Core.Domain.Entities;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Infrastructure.Repositories;

internal sealed class BookRepository(LibraryDbContext context) : IBookRepository
{
    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Books
            .AsNoTracking()   
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Books.FindAsync([id], cancellationToken);
    }

    public async Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        context.Books.Add(book);
        await context.SaveChangesAsync(cancellationToken);
        
        return book;
    }

    public async Task<Book> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        context.Books.Update(book);
        await context.SaveChangesAsync(cancellationToken);
        
        return book;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await GetByIdAsync(id, cancellationToken);
        if (book != null)
        {
            context.Books.Remove(book);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Books.AnyAsync(b => b.Id == id, cancellationToken);
    }
    
}