using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ExerciseCategoryConfiguration : IEntityTypeConfiguration<ExerciseCategory>
{
    public void Configure(EntityTypeBuilder<ExerciseCategory> builder)
    {
        builder.HasKey(ec => ec.Id);

        builder.Property(ec => ec.Sport)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(ec => ec.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ec => ec.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ec => ec.IsActive)
            .IsRequired();

        // Audit properties
        builder.Property(ec => ec.CreatedAt)
            .IsRequired();

        builder.Property(ec => ec.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ec => ec.UpdatedBy)
            .HasMaxLength(255);

        // Index
        builder.HasIndex(ec => new { ec.Sport, ec.IsActive })
            .HasDatabaseName("IX_ExerciseCategories_Sport_IsActive");

        builder.ToTable("exercise_categories");
    }
}