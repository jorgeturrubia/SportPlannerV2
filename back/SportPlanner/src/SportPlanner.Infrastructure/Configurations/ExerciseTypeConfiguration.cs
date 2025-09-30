using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ExerciseTypeConfiguration : IEntityTypeConfiguration<ExerciseType>
{
    public void Configure(EntityTypeBuilder<ExerciseType> builder)
    {
        builder.HasKey(et => et.Id);

        builder.Property(et => et.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(et => et.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(et => et.RequiresSets)
            .IsRequired();

        builder.Property(et => et.RequiresReps)
            .IsRequired();

        builder.Property(et => et.RequiresDuration)
            .IsRequired();

        builder.Property(et => et.IsActive)
            .IsRequired();

        // Audit properties
        builder.Property(et => et.CreatedAt)
            .IsRequired();

        builder.Property(et => et.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(et => et.UpdatedBy)
            .HasMaxLength(255);

        // Index
        builder.HasIndex(et => et.IsActive)
            .HasDatabaseName("IX_ExerciseTypes_IsActive");

        builder.ToTable("exercise_types");
    }
}