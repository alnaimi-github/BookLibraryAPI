using BookLibraryAPI.Application.Features.Users.Commands.Login;
using FluentAssertions;
using Xunit;

namespace BookLibraryAPI.UnitTests.Users;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var command = new LoginCommand("", "password123");
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = new LoginCommand("user", "");
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new LoginCommand("user", "password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
