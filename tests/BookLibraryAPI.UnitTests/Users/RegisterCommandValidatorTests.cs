using BookLibraryAPI.Application.Features.Users.Commands.Register;
using FluentAssertions;
using Xunit;

namespace BookLibraryAPI.UnitTests.Users;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var command = new RegisterCommand("", "password123", "User");
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = new RegisterCommand("user", "", "User");
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Should_Have_Error_When_Role_Is_Invalid()
    {
        var command = new RegisterCommand("user", "password123", "InvalidRole");
        var result = _validator.Validate(command);
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new RegisterCommand("user", "password123", "User");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
