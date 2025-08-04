using BookLibraryAPI.Core.Domain.Common.Exceptions;

namespace BookLibraryAPI.Core.Domain.ValueObjects;

public sealed record BookAuthor
{
    public string Value { get; }

    public BookAuthor(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Author cannot be empty.", nameof(value));
        if (value.Length > 100)
            throw new DomainException("Author cannot exceed 100 characters.", nameof(value));

        Value = value.Trim();
    }
    public static BookAuthor Create(string author)
    {
        return new BookAuthor(author);
    }
}