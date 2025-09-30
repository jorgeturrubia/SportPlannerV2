using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ExerciseObjectiveConfiguration : IEntityTypeConfiguration<ExerciseObjective>
{
    public void Configure(EntityTypeBuilder<ExerciseObjective> builder)
    {
        builder.HasKey(eo => new { eo.ExerciseId, eo.ObjectiveId });

        builder.Property(eo => eo.ExerciseId)
            .IsRequired();

        builder.Property(eo => eo.ObjectiveId)
            .IsRequired();

        // Relationships
        builder.HasOne(eo => eo.Exercise)
            .WithMany()
            .HasForeignKey(eo => eo.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(eo => eo.Objective)
            .WithMany()
            .HasForeignKey(eo => eo.ObjectiveId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for reverse lookup
        builder.HasIndex(eo => eo.ObjectiveId)
            .HasDatabaseName("IX_ExerciseObjectives_ObjectiveId");

        builder.ToTable("exercise_objectives");
    }
}