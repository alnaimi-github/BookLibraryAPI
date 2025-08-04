using BookLibraryAPI.Core.Domain.Entities;
using BookLibraryAPI.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibraryAPI.Infrastructure.Persistence.Configurations;

public sealed class ConfigurationBooks : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(
                v => v.Value,
                v => BookTitle.Create(v));

        builder.Property(e => e.Author)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                v => v.Value,
                v => BookAuthor.Create(v));
            
        
        builder.Property(e => e.Year)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt);

        builder.HasIndex("Title");
        builder.HasIndex("Author");
    }
}