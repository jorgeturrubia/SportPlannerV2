using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Infrastructure.Configurations;

public class TrainingPlanConfiguration : IEntityTypeConfiguration<TrainingPlan>
{
    public void Configure(EntityTypeBuilder<TrainingPlan> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.SubscriptionId)
            .IsRequired();

        builder.Property(tp => tp.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(tp => tp.StartDate)
            .IsRequired();

        builder.Property(tp => tp.EndDate)
            .IsRequired();

        builder.Property(tp => tp.IsActive)
            .IsRequired();

        builder.Property(tp => tp.MarketplaceItemId)
            .IsRequired(false);

        // Audit properties
        builder.Property(tp => tp.CreatedAt)
            .IsRequired();

        builder.Property(tp => tp.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(tp => tp.UpdatedBy)
            .HasMaxLength(255);

        // Configure TrainingSchedule as owned type (JSON storage)
        builder.OwnsOne(tp => tp.Schedule, schedule =>
        {
            var trainingDaysComparer = new ValueComparer<DayOfWeek[]>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray());

            var hoursPerDayComparer = new ValueComparer<Dictionary<DayOfWeek, int>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any()),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary(entry => entry.Key, entry => entry.Value));

            schedule.Property(s => s.TrainingDays)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<DayOfWeek[]>(v) ?? Array.Empty<DayOfWeek>())
                .Metadata.SetValueComparer(trainingDaysComparer);
            schedule.Property(s => s.TrainingDays)
                .HasColumnName("training_days")
                .IsRequired();

            schedule.Property(s => s.HoursPerDay)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<DayOfWeek, int>>(v) ?? new Dictionary<DayOfWeek, int>())
                .Metadata.SetValueComparer(hoursPerDayComparer);
            schedule.Property(s => s.HoursPerDay)
                .HasColumnName("hours_per_day")
                .IsRequired();

            schedule.Property(s => s.TotalWeeks)
                .HasColumnName("total_weeks")
                .IsRequired();
        });

        // Relationships
        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(tp => tp.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(tp => tp.Objectives)
            .WithOne(po => po.TrainingPlan)
            .HasForeignKey(po => po.TrainingPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(tp => new { tp.SubscriptionId, tp.IsActive })
            .HasDatabaseName("IX_TrainingPlans_SubscriptionId_IsActive");

        builder.HasIndex(tp => tp.SubscriptionId)
            .HasDatabaseName("IX_TrainingPlans_SubscriptionId");

        builder.HasIndex(tp => tp.MarketplaceItemId)
            .HasDatabaseName("IX_TrainingPlans_MarketplaceItemId");

        // Table name
        builder.ToTable("training_plans");
    }
}