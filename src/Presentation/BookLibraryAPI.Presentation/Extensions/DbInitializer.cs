using BookLibraryAPI.Core.Domain.Entities;
using BookLibraryAPI.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibraryAPI.Presentation.Extensions;

public static class DbInitializer
{
    public static void Seed(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

        if (!context.Books.Any())
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
        }
    }
}