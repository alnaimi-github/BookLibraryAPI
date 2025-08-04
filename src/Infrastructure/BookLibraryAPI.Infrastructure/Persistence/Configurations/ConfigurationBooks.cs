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
                title => title.Value,
                value => BookTitle.Create(value))
            .HasColumnName("Title");

        builder.Property(e => e.Author)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                author => author.Value,
                value => BookAuthor.Create(value))
            .HasColumnName("Author");
            
        
        builder.Property(e => e.Year)
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt);

        builder.HasIndex("Title");
        builder.HasIndex("Author");
        
   }

}