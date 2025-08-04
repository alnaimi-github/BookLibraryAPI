using BookLibraryAPI.Core.Domain.Common;

namespace BookLibraryAPI.Core.Domain.Entities;

public sealed class Book : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public int Year { get; private set; }
    
    public static Book Create(string title, string author, int year)
    {
        return new Book
        {
            Title = title.Trim(),
            Author = author.Trim(),
            Year = year
        };
    }
    public void Update(string title, string author, int year)
    {
        Title = title.Trim();
        Author = author.Trim();
        Year = year;
        SetUpdatedAt();
    }
}