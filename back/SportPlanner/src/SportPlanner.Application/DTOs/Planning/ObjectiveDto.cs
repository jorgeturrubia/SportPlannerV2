using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class ObjectiveDto
{
    public Guid Id { get; set; }
    public Guid? SubscriptionId { get; set; }
    public ContentOwnership Ownership { get; set; }
    public Sport Sport { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid ObjectiveCategoryId { get; set; }
    public Guid? ObjectiveSubcategoryId { get; set; }
    // Human-readable names (populated by handlers/controllers)
    public string? ObjectiveCategoryName { get; set; }
    public string? ObjectiveSubcategoryName { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public Guid? SourceMarketplaceItemId { get; set; }
    public List<ObjectiveTechniqueDto> Techniques { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // For convenience
    public bool IsSystemContent => Ownership == ContentOwnership.System;
    public bool IsUserContent => Ownership == ContentOwnership.User;
    public bool IsMarketplaceContent => Ownership == ContentOwnership.MarketplaceUser;
    public bool IsEditable => Ownership == ContentOwnership.User;
}