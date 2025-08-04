using FluentValidation;

namespace BookLibraryAPI.Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Book ID must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author cannot exceed 100 characters.");

        RuleFor(x => x.Year)
            .InclusiveBetween(0, 2100).WithMessage("Year must be between 0 and 2100.");
    }
}

