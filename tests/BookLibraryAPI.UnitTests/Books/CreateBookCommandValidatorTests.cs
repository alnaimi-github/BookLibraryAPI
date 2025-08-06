using BookLibraryAPI.Application.Features.Books.Commands.CreateBook;
using FluentAssertions;
using Xunit;

namespace BookLibraryAPI.UnitTests;

public class CreateBookCommandValidatorTests
{
    private readonly CreateBookCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateBookCommand("", "Author", 2024);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Should_Have_Error_When_Author_Is_Empty()
    {
        var command = new CreateBookCommand("Title", "", 2024);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author");
    }

    [Fact]
    public void Should_Have_Error_When_Year_Is_Invalid()
    {
        var command = new CreateBookCommand("Title", "Author", 999);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = new CreateBookCommand("Valid Title", "Valid Author", 2024);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
