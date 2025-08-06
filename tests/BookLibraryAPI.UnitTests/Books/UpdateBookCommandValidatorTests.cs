using BookLibraryAPI.Application.Features.Books.Commands.UpdateBook;
using FluentAssertions;
using Xunit;

namespace BookLibraryAPI.UnitTests.Books;

public class UpdateBookCommandValidatorTests
{
    private readonly UpdateBookCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new UpdateBookCommand { Id = 1, Title = "", Author = "Author", Year = 2020 };
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Should_Have_Error_When_Author_Is_Empty()
    {
        var command = new UpdateBookCommand { Id = 1, Title = "Title", Author = "", Year = 2020 };
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Author");
    }

    [Fact]
    public void Should_Have_Error_When_Year_Is_Invalid()
    {
        var command = new UpdateBookCommand { Id = 1, Title = "Title", Author = "Author", Year = 2200 };
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new UpdateBookCommand { Id = 1, Title = "Valid Title", Author = "Valid Author", Year = 2024 };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
