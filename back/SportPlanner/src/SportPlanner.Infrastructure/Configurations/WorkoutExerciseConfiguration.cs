using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.HasKey(we => new { we.WorkoutId, we.ExerciseId });

        builder.Property(we => we.WorkoutId)
            .IsRequired();

        builder.Property(we => we.ExerciseId)
            .IsRequired();

        builder.Property(we => we.Order)
            .IsRequired();

        builder.Property(we => we.Intensity)
            .HasMaxLength(50);

        builder.Property(we => we.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(we => we.Workout)
            .WithMany(w => w.Exercises)
            .HasForeignKey(we => we.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(we => we.Exercise)
            .WithMany()
            .HasForeignKey(we => we.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for exercise lookup
        builder.HasIndex(we => we.ExerciseId)
            .HasDatabaseName("IX_WorkoutExercises_ExerciseId");

        // Index for ordering
        builder.HasIndex(we => new { we.WorkoutId, we.Order })
            .HasDatabaseName("IX_WorkoutExercises_WorkoutId_Order");

        builder.ToTable("workout_exercises");
    }
}