namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Represents a specific technique or skill point within an objective.
/// Owned entity - cannot exist without parent Objective.
/// </summary>
public class ObjectiveTechnique
{
    public string Description { get; private set; }
    public int Order { get; private set; }

    // For EF Core
    private ObjectiveTechnique() { }

    public ObjectiveTechnique(string description, int order)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));

        Description = description;
        Order = order;
    }

    public void Update(string description, int order)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (order < 0)
            throw new ArgumentException("Order must be non-negative", nameof(order));

        Description = description;
        Order = order;
    }
}