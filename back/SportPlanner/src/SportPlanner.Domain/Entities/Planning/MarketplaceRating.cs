using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a rating given by a subscription to a marketplace item.
/// One subscription can only rate an item once.
/// </summary>
public class MarketplaceRating : IAuditable
{
    public Guid MarketplaceItemId { get; private set; }
    public Guid RatedBySubscriptionId { get; private set; }
    public int Stars { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation property
    public MarketplaceItem MarketplaceItem { get; private set; } = null!;

    // For EF Core
    private MarketplaceRating()
    {
        CreatedBy = string.Empty;
    }

    public MarketplaceRating(
        Guid marketplaceItemId,
        Guid ratedBySubscriptionId,
        int stars,
        string? comment,
        string createdBy)
    {
        if (marketplaceItemId == Guid.Empty)
            throw new ArgumentException("Marketplace item ID cannot be empty", nameof(marketplaceItemId));

        if (ratedBySubscriptionId == Guid.Empty)
            throw new ArgumentException("Rated by subscription ID cannot be empty", nameof(ratedBySubscriptionId));

        if (stars < 1 || stars > 5)
            throw new ArgumentException("Stars must be between 1 and 5", nameof(stars));

        if (comment != null && comment.Length > 1000)
            throw new ArgumentException("Comment cannot exceed 1000 characters", nameof(comment));

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));

        MarketplaceItemId = marketplaceItemId;
        RatedBySubscriptionId = ratedBySubscriptionId;
        Stars = stars;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void Update(int stars, string? comment)
    {
        if (stars < 1 || stars > 5)
            throw new ArgumentException("Stars must be between 1 and 5", nameof(stars));

        if (comment != null && comment.Length > 1000)
            throw new ArgumentException("Comment cannot exceed 1000 characters", nameof(comment));

        Stars = stars;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }
}