using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.SubscriptionId)
            .IsRequired(false); // NULL for System content

        builder.Property(w => w.Ownership)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(w => w.MarketplaceUserId)
            .IsRequired(false);

        builder.Property(w => w.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(w => w.ObjectiveId)
            .IsRequired(false);

        builder.Property(w => w.Difficulty)
            .HasMaxLength(50);

        builder.Property(w => w.Notes)
            .HasMaxLength(2000);

        builder.Property(w => w.IsActive)
            .IsRequired();

        // Audit properties
        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.UpdatedBy)
            .HasMaxLength(255);

        // Relationships
        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(w => w.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(w => w.Objective)
            .WithMany()
            .HasForeignKey(w => w.ObjectiveId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Indices for performance
        builder.HasIndex(w => new { w.SubscriptionId, w.IsActive })
            .HasDatabaseName("IX_Workouts_SubscriptionId_IsActive");

        builder.HasIndex(w => new { w.Ownership, w.IsActive })
            .HasDatabaseName("IX_Workouts_Ownership_IsActive")
            .HasFilter("subscription_id IS NULL");

        builder.HasIndex(w => w.ObjectiveId)
            .HasDatabaseName("IX_Workouts_ObjectiveId");

        builder.ToTable("workouts");
    }
}