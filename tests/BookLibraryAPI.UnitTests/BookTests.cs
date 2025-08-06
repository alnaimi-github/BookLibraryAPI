using BookLibraryAPI.Core.Domain.Books;
using FluentAssertions;
using Xunit;

namespace BookLibraryAPI.UnitTests;

public class BookTests
{
    [Fact]
    public void Create_ShouldReturnBook_WithCorrectProperties()
    {
        // Arrange
        var title = "Test Title";
        var author = "Test Author";
        var year = 2024;

        // Act
        var book = Book.Create(title, author, year);

        // Assert
        book.Title.Value.Should().Be(title);
        book.Author.Value.Should().Be(author);
        book.Year.Should().Be(year);
    }

    [Fact]
    public void Update_ShouldChangeBookProperties()
    {
        // Arrange
        var book = Book.Create("Old Title", "Old Author", 2000);
        var newTitle = "New Title";
        var newAuthor = "New Author";
        var newYear = 2025;

        // Act
        book.Update(newTitle, newAuthor, newYear);

        // Assert
        book.Title.Value.Should().Be(newTitle);
        book.Author.Value.Should().Be(newAuthor);
        book.Year.Should().Be(newYear);
    }
}
