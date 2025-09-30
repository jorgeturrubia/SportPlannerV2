namespace SportPlanner.Domain.Entities.Planning;

public class ExerciseType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool RequiresSets { get; private set; }
    public bool RequiresReps { get; private set; }
    public bool RequiresDuration { get; private set; }
    public bool IsActive { get; private set; }

    // Audit
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private ExerciseType() { }

    public ExerciseType(
        Guid id,
        string name,
        string description,
        bool requiresSets,
        bool requiresReps,
        bool requiresDuration,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Id = id;
        Name = name;
        Description = description;
        RequiresSets = requiresSets;
        RequiresReps = requiresReps;
        RequiresDuration = requiresDuration;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}