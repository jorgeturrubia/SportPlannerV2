using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class PlanObjectiveConfiguration : IEntityTypeConfiguration<PlanObjective>
{
    public void Configure(EntityTypeBuilder<PlanObjective> builder)
    {
        // Composite key
        builder.HasKey(po => new { po.TrainingPlanId, po.ObjectiveId });

        builder.Property(po => po.Priority)
            .IsRequired();

        builder.Property(po => po.TargetSessions)
            .IsRequired();

        // Relationships
        builder.HasOne(po => po.TrainingPlan)
            .WithMany(tp => tp.Objectives)
            .HasForeignKey(po => po.TrainingPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(po => po.Objective)
            .WithMany()
            .HasForeignKey(po => po.ObjectiveId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete objective if used in plan

        // Indices
        builder.HasIndex(po => po.TrainingPlanId)
            .HasDatabaseName("IX_PlanObjectives_TrainingPlanId");

        builder.HasIndex(po => po.ObjectiveId)
            .HasDatabaseName("IX_PlanObjectives_ObjectiveId");

        builder.HasIndex(po => new { po.TrainingPlanId, po.Priority })
            .HasDatabaseName("IX_PlanObjectives_TrainingPlanId_Priority");

        // Table name
        builder.ToTable("plan_objectives");
    }
}