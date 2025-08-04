using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.ValueObjects;

namespace BookLibraryAPI.Core.Domain.Entities;

public sealed class Book : BaseEntity
{
    public BookTitle Title { get; private set; } 
    public BookAuthor Author { get; private set; }
    public int Year { get; private set; }
    
    private Book() { } // EF Core requires a parameterless constructor
    
    public static Book Create(string title, string author, int year)
    {
        return new Book
        {
            Title = BookTitle.Create(title),
            Author = BookAuthor.Create(author),
            Year = year
        };
    }
    public void Update(string title, string author, int year)
    {
        Title = BookTitle.Create(title);
        Author = BookAuthor.Create(author);
        Year = year;
        SetUpdatedAt();
    }
}