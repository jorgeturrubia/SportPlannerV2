using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.SubscriptionId)
            .IsRequired(false); // NULL for System content

        builder.Property(e => e.Ownership)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.MarketplaceUserId)
            .IsRequired(false);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.CategoryId)
            .IsRequired();

        builder.Property(e => e.TypeId)
            .IsRequired();

        builder.Property(e => e.VideoUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Instructions)
            .HasMaxLength(2000);

        builder.Property(e => e.DefaultIntensity)
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired();

        // Audit properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(255);

        // Relationships
        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(e => e.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Type)
            .WithMany()
            .HasForeignKey(e => e.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices for performance
        builder.HasIndex(e => new { e.SubscriptionId, e.IsActive })
            .HasDatabaseName("IX_Exercises_SubscriptionId_IsActive");

        builder.HasIndex(e => new { e.Ownership, e.IsActive })
            .HasDatabaseName("IX_Exercises_Ownership_IsActive")
            .HasFilter("subscription_id IS NULL");

        builder.HasIndex(e => e.CategoryId)
            .HasDatabaseName("IX_Exercises_CategoryId");

        builder.HasIndex(e => e.TypeId)
            .HasDatabaseName("IX_Exercises_TypeId");

        builder.ToTable("exercises");
    }
}