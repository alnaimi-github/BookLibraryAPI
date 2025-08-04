using BookLibraryAPI.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLibraryAPI.Infrastructure.Persistence.Configurations.Users;

public sealed class ConfigurationUsers : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.PasswordHash)
            .IsRequired();
        
        builder.Property(e => e.Role)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt);

        builder.HasIndex(e => e.Username).IsUnique();
    }
}