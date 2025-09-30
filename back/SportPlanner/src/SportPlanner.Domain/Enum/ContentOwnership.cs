namespace SportPlanner.Domain.Enum;

/// <summary>
/// Defines the ownership type of content in the system.
/// </summary>
public enum ContentOwnership
{
    /// <summary>
    /// Content created by a user (editable by owner).
    /// </summary>
    User,

    /// <summary>
    /// Official platform content (read-only, pre-seeded).
    /// </summary>
    System,

    /// <summary>
    /// Content downloaded from another user via marketplace (read-only, can be cloned to User).
    /// </summary>
    MarketplaceUser
}