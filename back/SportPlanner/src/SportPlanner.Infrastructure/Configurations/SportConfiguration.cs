using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Infrastructure.Configurations;

public class SportConfiguration : IEntityTypeConfiguration<Sport>
{
    public void Configure(EntityTypeBuilder<Sport> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.SortOrder)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        // Audit properties
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.UpdatedBy)
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(s => s.Code)
            .IsUnique()
            .HasDatabaseName("IX_Sports_Code");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("IX_Sports_Name");

        // Table name
        builder.ToTable("sports");
    }
}
