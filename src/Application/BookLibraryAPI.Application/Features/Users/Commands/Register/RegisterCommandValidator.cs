using BookLibraryAPI.Core.Domain.Users.Enums;
using FluentValidation;

namespace BookLibraryAPI.Application.Features.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => Enum.GetNames(typeof(UserRole)).Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Role must be Admin, Moderator, or User.");
    }
}
