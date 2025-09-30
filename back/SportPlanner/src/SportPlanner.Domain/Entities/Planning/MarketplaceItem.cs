using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a content item published in the marketplace.
/// Can be Objective, Exercise, Workout, or TrainingPlan.
/// Polymorphic entity that references the actual source entity.
/// </summary>
public class MarketplaceItem : Entity, IAuditable
{
    public MarketplaceItemType Type { get; private set; }
    public Sport Sport { get; private set; }

    public Guid? SourceEntityId { get; private set; }
    public ContentOwnership SourceOwnership { get; private set; }
    public Guid? PublishedBySubscriptionId { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public bool IsSystemOfficial { get; private set; }
    public decimal AverageRating { get; private set; }
    public int TotalRatings { get; private set; }
    public int TotalDownloads { get; private set; }
    public int TotalViews { get; private set; }

    public DateTime PublishedAt { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    private readonly List<MarketplaceRating> _ratings = new();
    public IReadOnlyCollection<MarketplaceRating> Ratings => _ratings.AsReadOnly();

    // For EF Core
    private MarketplaceItem()
    {
        Name = string.Empty;
        Description = string.Empty;
        CreatedBy = string.Empty;
    }

    /// <summary>
    /// Creates a marketplace item for system official content.
    /// </summary>
    public static MarketplaceItem CreateSystemItem(
        MarketplaceItemType type,
        Sport sport,
        Guid sourceEntityId,
        string name,
        string description,
        string createdBy)
    {
        if (sourceEntityId == Guid.Empty)
            throw new ArgumentException("Source entity ID cannot be empty", nameof(sourceEntityId));

        ValidateCommonFields(name, description);

        return new MarketplaceItem
        {
            Type = type,
            Sport = sport,
            SourceEntityId = sourceEntityId,
            SourceOwnership = ContentOwnership.System,
            PublishedBySubscriptionId = null,
            Name = name,
            Description = description,
            IsSystemOfficial = true,
            AverageRating = 0,
            TotalRatings = 0,
            TotalDownloads = 0,
            TotalViews = 0,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Creates a marketplace item for user-generated content.
    /// </summary>
    public static MarketplaceItem CreateUserItem(
        MarketplaceItemType type,
        Sport sport,
        Guid sourceEntityId,
        Guid publishedBySubscriptionId,
        string name,
        string description,
        string createdBy)
    {
        if (sourceEntityId == Guid.Empty)
            throw new ArgumentException("Source entity ID cannot be empty", nameof(sourceEntityId));

        if (publishedBySubscriptionId == Guid.Empty)
            throw new ArgumentException("Published by subscription ID cannot be empty", nameof(publishedBySubscriptionId));

        ValidateCommonFields(name, description);

        return new MarketplaceItem
        {
            Type = type,
            Sport = sport,
            SourceEntityId = sourceEntityId,
            SourceOwnership = ContentOwnership.User,
            PublishedBySubscriptionId = publishedBySubscriptionId,
            Name = name,
            Description = description,
            IsSystemOfficial = false,
            AverageRating = 0,
            TotalRatings = 0,
            TotalDownloads = 0,
            TotalViews = 0,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void IncrementViews()
    {
        TotalViews++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementDownloads()
    {
        TotalDownloads++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRating(Guid ratedBySubscriptionId, int stars, string? comment, string createdBy)
    {
        if (ratedBySubscriptionId == Guid.Empty)
            throw new ArgumentException("Rated by subscription ID cannot be empty", nameof(ratedBySubscriptionId));

        if (stars < 1 || stars > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(stars));

        // Check if subscription already rated
        var existingRating = _ratings.FirstOrDefault(r => r.RatedBySubscriptionId == ratedBySubscriptionId);
        if (existingRating != null)
            throw new InvalidOperationException("Subscription has already rated this item");

        var rating = new MarketplaceRating(Id, ratedBySubscriptionId, stars, comment, createdBy);
        _ratings.Add(rating);

        RecalculateAverageRating();
    }

    public void UpdateRating(Guid ratedBySubscriptionId, int stars, string? comment)
    {
        if (stars < 1 || stars > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(stars));

        var rating = _ratings.FirstOrDefault(r => r.RatedBySubscriptionId == ratedBySubscriptionId);
        if (rating == null)
            throw new InvalidOperationException("Rating not found for this subscription");

        rating.Update(stars, comment);
        RecalculateAverageRating();
    }

    public void RemoveRating(Guid ratedBySubscriptionId)
    {
        var rating = _ratings.FirstOrDefault(r => r.RatedBySubscriptionId == ratedBySubscriptionId);
        if (rating == null)
            throw new InvalidOperationException("Rating not found for this subscription");

        _ratings.Remove(rating);
        RecalculateAverageRating();
    }

    private void RecalculateAverageRating()
    {
        if (_ratings.Count == 0)
        {
            AverageRating = 0;
            TotalRatings = 0;
        }
        else
        {
            AverageRating = Math.Round((decimal)_ratings.Average(r => r.Stars), 2);
            TotalRatings = _ratings.Count;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description, string updatedBy)
    {
        ValidateCommonFields(name, description);

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    private static void ValidateCommonFields(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(description));
    }
}