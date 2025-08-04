using BookLibraryAPI.Core.Domain.Common;

namespace BookLibraryAPI.Core.Domain.Entities;

public sealed class Book : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public int Year { get; private set; }
    
}