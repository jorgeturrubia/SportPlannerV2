using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class WorkoutObjectiveConfiguration : IEntityTypeConfiguration<WorkoutObjective>
{
    public void Configure(EntityTypeBuilder<WorkoutObjective> builder)
    {
        builder.HasKey(wo => new { wo.WorkoutId, wo.ObjectiveId });

        builder.Property(wo => wo.WorkoutId)
            .IsRequired();

        builder.Property(wo => wo.ObjectiveId)
            .IsRequired();

        // Relationships
        builder.HasOne(wo => wo.Workout)
            .WithMany()
            .HasForeignKey(wo => wo.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wo => wo.Objective)
            .WithMany()
            .HasForeignKey(wo => wo.ObjectiveId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for reverse lookup
        builder.HasIndex(wo => wo.ObjectiveId)
            .HasDatabaseName("IX_WorkoutObjectives_ObjectiveId");

        builder.ToTable("workout_objectives");
    }
}