using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Infrastructure.Configurations;

public class MarketplaceRatingConfiguration : IEntityTypeConfiguration<MarketplaceRating>
{
    public void Configure(EntityTypeBuilder<MarketplaceRating> builder)
    {
        builder.HasKey(mr => new { mr.MarketplaceItemId, mr.RatedBySubscriptionId });

        builder.Property(mr => mr.MarketplaceItemId)
            .IsRequired();

        builder.Property(mr => mr.RatedBySubscriptionId)
            .IsRequired();

        builder.Property(mr => mr.Stars)
            .IsRequired();

        builder.Property(mr => mr.Comment)
            .HasMaxLength(1000);

        // Audit properties
        builder.Property(mr => mr.CreatedAt)
            .IsRequired();

        builder.Property(mr => mr.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(mr => mr.UpdatedBy)
            .HasMaxLength(255);

        // Relationships
        builder.HasOne(mr => mr.MarketplaceItem)
            .WithMany(mi => mi.Ratings)
            .HasForeignKey(mr => mr.MarketplaceItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Subscription>()
            .WithMany()
            .HasForeignKey(mr => mr.RatedBySubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for subscription's ratings lookup
        builder.HasIndex(mr => mr.RatedBySubscriptionId)
            .HasDatabaseName("IX_MarketplaceRatings_RatedBySubscriptionId");

        builder.ToTable("marketplace_ratings");
    }
}