using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Master data entity representing objective subcategories.
/// Examples: Attack, Defense, Transition.
/// Always linked to a parent ObjectiveCategory.
/// </summary>
public class ObjectiveSubcategory : Entity, IAuditable
{
    public Guid ObjectiveCategoryId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation property
    public ObjectiveCategory Category { get; private set; }

    // For EF Core
    private ObjectiveSubcategory() { }

    public ObjectiveSubcategory(Guid categoryId, string name)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty", nameof(categoryId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        ObjectiveCategoryId = categoryId;
        Name = name;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}