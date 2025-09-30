namespace SportPlanner.Domain.Enum;

/// <summary>
/// Filter options for browsing marketplace content.
/// </summary>
public enum MarketplaceFilter
{
    /// <summary>
    /// Show all content (official + community).
    /// </summary>
    All,

    /// <summary>
    /// Show only official system content.
    /// </summary>
    OfficialOnly,

    /// <summary>
    /// Show only community-generated content.
    /// </summary>
    CommunityOnly,

    /// <summary>
    /// Show most downloaded/popular content.
    /// </summary>
    Popular,

    /// <summary>
    /// Show highest rated content.
    /// </summary>
    TopRated
}