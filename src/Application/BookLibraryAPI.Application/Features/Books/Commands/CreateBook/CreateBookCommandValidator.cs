using FluentValidation;

namespace BookLibraryAPI.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .Must(title => !string.IsNullOrWhiteSpace(title)).WithMessage("Title cannot be only whitespace");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters")
            .Must(author => !string.IsNullOrWhiteSpace(author)).WithMessage("Author cannot be only whitespace");

        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Year must be greater than 0")
            .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Year cannot be in the future")
            .GreaterThanOrEqualTo(1000).WithMessage("Year must be realistic");
    }
}