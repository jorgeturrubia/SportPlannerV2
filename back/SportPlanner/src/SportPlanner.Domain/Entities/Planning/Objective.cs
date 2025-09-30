using SportPlanner.Domain.Enum;
using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a training objective (e.g., "Improve hand change", "Free throw under pressure").
/// Can be System content (read-only, SubscriptionId = NULL) or User content (editable).
/// </summary>
public class Objective : Entity, IAuditable
{
    public Guid? SubscriptionId { get; private set; }
    public ContentOwnership Ownership { get; private set; }
    public Sport Sport { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid ObjectiveCategoryId { get; private set; }
    public Guid? ObjectiveSubcategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? SourceMarketplaceItemId { get; private set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ObjectiveCategory Category { get; private set; }
    public ObjectiveSubcategory? Subcategory { get; private set; }

    private readonly List<ObjectiveTechnique> _techniques = new();
    public IReadOnlyCollection<ObjectiveTechnique> Techniques => _techniques.AsReadOnly();

    // For EF Core
    private Objective() { }

    /// <summary>
    /// Creates a user-owned objective.
    /// </summary>
    public Objective(
        Guid subscriptionId,
        Sport sport,
        string name,
        string description,
        Guid categoryId,
        Guid? subcategoryId = null)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty for user content", nameof(subscriptionId));

        ValidateCommonFields(name, description, categoryId, sport);

        SubscriptionId = subscriptionId;
        Ownership = ContentOwnership.User;
        Sport = sport;
        Name = name;
        Description = description;
        ObjectiveCategoryId = categoryId;
        ObjectiveSubcategoryId = subcategoryId;
        IsActive = true;
    }

    /// <summary>
    /// Creates system-owned objective (for seed data).
    /// </summary>
    public static Objective CreateSystemObjective(
        Sport sport,
        string name,
        string description,
        Guid categoryId,
        Guid? subcategoryId = null)
    {
        ValidateCommonFields(name, description, categoryId, sport);

        return new Objective
        {
            SubscriptionId = null,
            Ownership = ContentOwnership.System,
            Sport = sport,
            Name = name,
            Description = description,
            ObjectiveCategoryId = categoryId,
            ObjectiveSubcategoryId = subcategoryId,
            IsActive = true
        };
    }

    /// <summary>
    /// Creates a marketplace-downloaded objective (read-only copy).
    /// </summary>
    public static Objective CreateFromMarketplace(
        Guid subscriptionId,
        Guid sourceMarketplaceItemId,
        Sport sport,
        string name,
        string description,
        Guid categoryId,
        Guid? subcategoryId = null)
    {
        if (subscriptionId == Guid.Empty)
            throw new ArgumentException("SubscriptionId cannot be empty", nameof(subscriptionId));

        if (sourceMarketplaceItemId == Guid.Empty)
            throw new ArgumentException("SourceMarketplaceItemId cannot be empty", nameof(sourceMarketplaceItemId));

        ValidateCommonFields(name, description, categoryId, sport);

        return new Objective
        {
            SubscriptionId = subscriptionId,
            Ownership = ContentOwnership.MarketplaceUser,
            Sport = sport,
            Name = name,
            Description = description,
            ObjectiveCategoryId = categoryId,
            ObjectiveSubcategoryId = subcategoryId,
            SourceMarketplaceItemId = sourceMarketplaceItemId,
            IsActive = true
        };
    }

    /// <summary>
    /// Clones this objective as User content (from System or MarketplaceUser).
    /// </summary>
    public Objective Clone(Guid targetSubscriptionId)
    {
        if (targetSubscriptionId == Guid.Empty)
            throw new ArgumentException("Target subscription cannot be empty", nameof(targetSubscriptionId));

        if (Ownership == ContentOwnership.User)
            throw new InvalidOperationException("Cannot clone user content directly. Use standard creation instead.");

        var cloned = new Objective(
            targetSubscriptionId,
            Sport,
            Name,
            Description,
            ObjectiveCategoryId,
            ObjectiveSubcategoryId);

        // Clone techniques
        foreach (var technique in _techniques.OrderBy(t => t.Order))
        {
            cloned.AddTechnique(technique.Description, technique.Order);
        }

        return cloned;
    }

    public void Update(string name, string description, Guid categoryId, Guid? subcategoryId = null)
    {
        EnsureIsEditable();
        ValidateCommonFields(name, description, categoryId, Sport);

        Name = name;
        Description = description;
        ObjectiveCategoryId = categoryId;
        ObjectiveSubcategoryId = subcategoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTechnique(string description, int order)
    {
        EnsureIsEditable();

        var technique = new ObjectiveTechnique(description, order);
        _techniques.Add(technique);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTechniques(List<(string Description, int Order)> techniques)
    {
        EnsureIsEditable();

        _techniques.Clear();
        foreach (var (description, order) in techniques)
        {
            _techniques.Add(new ObjectiveTechnique(description, order));
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        EnsureIsEditable();
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        EnsureIsEditable();
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsSystemContent() => Ownership == ContentOwnership.System;
    public bool IsUserContent() => Ownership == ContentOwnership.User;
    public bool IsMarketplaceContent() => Ownership == ContentOwnership.MarketplaceUser;
    public bool IsEditable() => Ownership == ContentOwnership.User;

    private void EnsureIsEditable()
    {
        if (!IsEditable())
            throw new InvalidOperationException($"Cannot modify {Ownership} content. Clone it first to make changes.");
    }

    private static void ValidateCommonFields(string name, string description, Guid categoryId, Sport sport)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(description));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty", nameof(categoryId));
    }
}