using SportPlanner.Domain.Interfaces;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Master data entity representing high-level objective categories.
/// Examples: Individual Technique, Collective Technique, Physical, Tactical.
/// </summary>
public class ObjectiveCategory : Entity, IAuditable
{
    public string Name { get; private set; }
    public Guid SportId { get; private set; }
    public Sport Sport { get; private set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // For EF Core
    private ObjectiveCategory() { }

    public ObjectiveCategory(string name, Guid sportId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (sportId == Guid.Empty)
            throw new ArgumentException("SportId cannot be empty", nameof(sportId));

        Name = name;
        SportId = sportId;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSport(Guid sportId)
    {
        if (sportId == Guid.Empty)
            throw new ArgumentException("SportId cannot be empty", nameof(sportId));

        SportId = sportId;
        UpdatedAt = DateTime.UtcNow;
    }
}