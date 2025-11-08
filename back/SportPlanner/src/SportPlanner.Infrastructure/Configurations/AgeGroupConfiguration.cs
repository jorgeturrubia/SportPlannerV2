using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Infrastructure.Configurations;

public class AgeGroupConfiguration : IEntityTypeConfiguration<AgeGroup>
{
    public void Configure(EntityTypeBuilder<AgeGroup> builder)
    {
        builder.HasKey(ag => ag.Id);

        builder.Property(ag => ag.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ag => ag.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ag => ag.MinAge)
            .IsRequired();

        builder.Property(ag => ag.MaxAge)
            .IsRequired();

        builder.Property(ag => ag.SportId)
            .IsRequired();

        // EF Core will automatically configure the relationship based on naming convention
        // SportId -> Sport navigation property

        builder.Property(ag => ag.SortOrder)
            .IsRequired();

        builder.Property(ag => ag.IsActive)
            .IsRequired();

        builder.Property(ag => ag.CreatedAt)
            .IsRequired();

        builder.Property(ag => ag.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ag => ag.UpdatedBy)
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(ag => ag.Code)
            .IsUnique();

        builder.HasIndex(ag => new { ag.SportId, ag.SortOrder });

        builder.HasIndex(ag => new { ag.SportId, ag.MinAge, ag.MaxAge });

        builder.HasIndex(ag => ag.IsActive);

        // Table name
        builder.ToTable("age_groups");
    }
}