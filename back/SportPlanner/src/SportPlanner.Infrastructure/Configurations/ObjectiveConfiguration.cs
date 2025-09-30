using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
{
    public void Configure(EntityTypeBuilder<Objective> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.SubscriptionId)
            .IsRequired(false); // NULL for System content

        builder.Property(o => o.Ownership)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Sport)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(o => o.ObjectiveCategoryId)
            .IsRequired();

        builder.Property(o => o.ObjectiveSubcategoryId)
            .IsRequired(false);

        builder.Property(o => o.IsActive)
            .IsRequired();

        builder.Property(o => o.SourceMarketplaceItemId)
            .IsRequired(false);

        // Audit properties
        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.UpdatedBy)
            .HasMaxLength(255);

        // Owned collection - ObjectiveTechniques
        builder.OwnsMany(o => o.Techniques, technique =>
        {
            technique.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();

            technique.Property(t => t.Order)
                .IsRequired();

            technique.ToTable("objective_techniques");
        });

        // Relationships
        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(o => o.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(o => o.Category)
            .WithMany()
            .HasForeignKey(o => o.ObjectiveCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Subcategory)
            .WithMany()
            .HasForeignKey(o => o.ObjectiveSubcategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Indices for performance
        // Index for user objectives by subscription
        builder.HasIndex(o => new { o.SubscriptionId, o.Sport, o.IsActive })
            .HasDatabaseName("IX_Objectives_SubscriptionId_Sport_IsActive");

        // Index for system content by sport
        builder.HasIndex(o => new { o.Ownership, o.Sport })
            .HasDatabaseName("IX_Objectives_Ownership_Sport")
            .HasFilter("subscription_id IS NULL");

        // Index for category lookup
        builder.HasIndex(o => o.ObjectiveCategoryId)
            .HasDatabaseName("IX_Objectives_ObjectiveCategoryId");

        // Index for subcategory lookup
        builder.HasIndex(o => o.ObjectiveSubcategoryId)
            .HasDatabaseName("IX_Objectives_ObjectiveSubcategoryId");

        // Index for marketplace sourced content
        builder.HasIndex(o => o.SourceMarketplaceItemId)
            .HasDatabaseName("IX_Objectives_SourceMarketplaceItemId");

        // Table name
        builder.ToTable("objectives");
    }
}