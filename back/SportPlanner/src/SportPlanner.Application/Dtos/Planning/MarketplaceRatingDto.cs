namespace SportPlanner.Application.Dtos.Planning;

/// <summary>
/// Data transfer object for a marketplace item rating.
/// </summary>
public record MarketplaceRatingDto(
    Guid RatedBySubscriptionId,
    int Stars,
    string? Comment,
    DateTime CreatedAt);