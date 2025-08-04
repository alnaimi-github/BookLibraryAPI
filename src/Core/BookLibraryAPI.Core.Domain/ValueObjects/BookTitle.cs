using BookLibraryAPI.Core.Domain.Common.Exceptions;

namespace BookLibraryAPI.Core.Domain.ValueObjects;

public sealed record BookTitle
{
    public string Value { get; }

    public BookTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Title cannot be empty.", nameof(value));
        if (value.Length > 200)
            throw new DomainException("Title cannot exceed 200 characters.", nameof(value));

        Value = value.Trim();
    }

    public static BookTitle Create(string title)
    {
        return new BookTitle(title);
    }
}
