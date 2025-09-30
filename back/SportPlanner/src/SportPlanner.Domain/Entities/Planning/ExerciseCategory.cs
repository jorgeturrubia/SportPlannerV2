using SportPlanner.Domain.Enum;

namespace SportPlanner.Domain.Entities.Planning;

/// <summary>
/// Master data entity representing high-level exercise categories.
/// Examples: Technical, Physical, Tactical, Specific.
/// </summary>
public class ExerciseCategory
{
    public Guid Id { get; private set; }
    public Sport Sport { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Audit
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private ExerciseCategory() { }

    public ExerciseCategory(Guid id, Sport sport, string name, string description, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Id = id;
        Sport = sport;
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}