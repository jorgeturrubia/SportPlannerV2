namespace SportPlanner.Application.DTOs.Planning;

public class ExerciseTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresSets { get; set; }
    public bool RequiresReps { get; set; }
    public bool RequiresDuration { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExerciseTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresSets { get; set; }
    public bool RequiresReps { get; set; }
    public bool RequiresDuration { get; set; }
}

public class UpdateExerciseTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresSets { get; set; }
    public bool RequiresReps { get; set; }
    public bool RequiresDuration { get; set; }
    public bool IsActive { get; set; }
}
