using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;

namespace SportPlanner.Infrastructure.Configurations;

public class TeamCategoryConfiguration : IEntityTypeConfiguration<TeamCategory>
{
    public void Configure(EntityTypeBuilder<TeamCategory> builder)
    {
        builder.HasKey(tc => tc.Id);

        builder.Property(tc => tc.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(tc => tc.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(tc => tc.Description)
            .HasMaxLength(500);

        builder.Property(tc => tc.SortOrder)
            .IsRequired();

        builder.Property(tc => tc.IsActive)
            .IsRequired();

        builder.Property(tc => tc.SportId)
            .IsRequired();

        // EF Core will automatically configure the relationship based on naming convention
        // SportId -> Sport navigation property

        builder.Property(tc => tc.CreatedAt)
            .IsRequired();

        builder.Property(tc => tc.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(tc => tc.UpdatedBy)
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(tc => tc.Code)
            .IsUnique();

        builder.HasIndex(tc => new { tc.Sport, tc.SortOrder });

        builder.HasIndex(tc => tc.IsActive);

        // Table name
        builder.ToTable("team_categories");
    }
}