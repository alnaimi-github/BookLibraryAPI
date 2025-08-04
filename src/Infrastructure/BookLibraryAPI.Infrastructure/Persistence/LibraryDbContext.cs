using BookLibraryAPI.Core.Domain.Books;
using BookLibraryAPI.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Infrastructure.Persistence;

public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options) 
    : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}