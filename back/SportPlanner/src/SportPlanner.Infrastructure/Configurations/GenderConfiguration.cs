using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Infrastructure.Configurations;

public class GenderConfiguration : IEntityTypeConfiguration<Gender>
{
    public void Configure(EntityTypeBuilder<Gender> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(g => g.Code)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(255);

        builder.Property(g => g.IsActive)
            .IsRequired();

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(g => g.UpdatedBy)
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(g => g.Code)
            .IsUnique();

        builder.HasIndex(g => g.IsActive);

        // Table name
        builder.ToTable("genders");
    }
}