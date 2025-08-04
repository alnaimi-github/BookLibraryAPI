using BookLibraryAPI.Core.Domain.Books;
using BookLibraryAPI.Core.Domain.Users;
using BookLibraryAPI.Core.Domain.Users.Enums;
using BookLibraryAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Presentation.Extensions;

public static class DbInitializer
{
    public static void Seed(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("DbInitializer");

        SeedBooks(context, logger);
        SeedUsers(context, logger);
    }

    private static void SeedBooks(LibraryDbContext context, ILogger? logger)
    {
        if (!context.Books.AsNoTracking().Any())
        {
            var books = new[]
            {
                Book.Create("The Great Gatsby", "F. Scott Fitzgerald", 1925),
                Book.Create("To Kill a Mockingbird", "Harper Lee", 1960),
                Book.Create("1984", "George Orwell", 1949),
                Book.Create("Pride and Prejudice", "Jane Austen", 1813),
                Book.Create("The Catcher in the Rye", "J.D. Salinger", 1951),
                Book.Create("The Lord of the Rings", "J.R.R. Tolkien", 1954),
                Book.Create("The Hobbit", "J.R.R. Tolkien", 1937),
                Book.Create("Fahrenheit 451", "Ray Bradbury", 1953),
                Book.Create("Brave New World", "Aldous Huxley", 1932),
                Book.Create("The Picture of Dorian Gray", "Oscar Wilde", 1890)
            };
            context.Books.AddRange(books);
            context.SaveChanges();
            logger?.LogInformation("Seeded initial books data.");
        }
    }

    private static void SeedUsers(LibraryDbContext context, ILogger? logger)
    {
        if (!context.Users.AsNoTracking().Any())
        {
            var users = new[]
            {
                User.Create("admin", BCrypt.Net.BCrypt.HashPassword("admin123"), UserRole.Admin),
                User.Create("moderator", BCrypt.Net.BCrypt.HashPassword("mod123"), UserRole.Moderator),
                User.Create("user", BCrypt.Net.BCrypt.HashPassword("user123"), UserRole.User)
            };
            context.Users.AddRange(users);
            context.SaveChanges();
            logger?.LogInformation("Seeded initial users data.");
        }
    }
}