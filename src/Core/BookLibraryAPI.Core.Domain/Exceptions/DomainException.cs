namespace BookLibraryAPI.Core.Domain.Exceptions;

public class DomainException : Exception
{
    public string Value { get; }

    public DomainException(string value, string message) : base($"{message} (Value: {value})")
    {
        Value = value;
    }
}
