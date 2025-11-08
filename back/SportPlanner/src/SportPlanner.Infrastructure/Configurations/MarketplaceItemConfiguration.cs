using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class MarketplaceItemConfiguration : IEntityTypeConfiguration<MarketplaceItem>
{
    public void Configure(EntityTypeBuilder<MarketplaceItem> builder)
    {
        builder.HasKey(mi => mi.Id);

        builder.Property(mi => mi.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(mi => mi.SportId)
            .IsRequired();

        builder.Property(mi => mi.SourceEntityId)
            .IsRequired(false);

        builder.Property(mi => mi.SourceOwnership)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(mi => mi.PublishedBySubscriptionId)
            .IsRequired(false);

        builder.Property(mi => mi.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(mi => mi.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(mi => mi.IsSystemOfficial)
            .IsRequired();

        builder.Property(mi => mi.AverageRating)
            .HasPrecision(3, 2)
            .IsRequired();

        builder.Property(mi => mi.TotalRatings)
            .IsRequired();

        builder.Property(mi => mi.TotalDownloads)
            .IsRequired();

        builder.Property(mi => mi.TotalViews)
            .IsRequired();

        builder.Property(mi => mi.PublishedAt)
            .IsRequired();

        // Audit properties
        builder.Property(mi => mi.CreatedAt)
            .IsRequired();

        builder.Property(mi => mi.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(mi => mi.UpdatedBy)
            .HasMaxLength(255);

        // Relationships
        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(mi => mi.PublishedBySubscriptionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Indices for performance (critical for ADR-003 search requirements)
        // Main index: search by sport and type with rating sort
        builder.HasIndex(mi => new { mi.SportId, mi.Type, mi.AverageRating })
            .HasDatabaseName("IX_MarketplaceItems_Sport_Type_Rating")
            .IsDescending(false, false, true);

        // Index for filtering official content
        builder.HasIndex(mi => new { mi.SportId, mi.IsSystemOfficial, mi.AverageRating })
            .HasDatabaseName("IX_MarketplaceItems_Sport_Official_Rating")
            .HasFilter("is_system_official = true")
            .IsDescending(false, false, true);

        // Index for popular content
        builder.HasIndex(mi => new { mi.SportId, mi.TotalDownloads })
            .HasDatabaseName("IX_MarketplaceItems_Sport_Downloads")
            .IsDescending(false, true);

        // Index for publisher lookup
        builder.HasIndex(mi => mi.PublishedBySubscriptionId)
            .HasDatabaseName("IX_MarketplaceItems_PublishedBySubscriptionId");

        // Index for source entity lookup
        builder.HasIndex(mi => mi.SourceEntityId)
            .HasDatabaseName("IX_MarketplaceItems_SourceEntityId");

        builder.ToTable("marketplace_items");
    }
}