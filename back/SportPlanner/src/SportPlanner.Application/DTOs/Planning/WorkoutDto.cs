using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.DTOs.Planning;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public Guid? SubscriptionId { get; set; }
    public ContentOwnership Ownership { get; set; }
    public DateTime Fecha { get; set; }
    public Guid? ObjectiveId { get; set; }
    public string? ObjectiveName { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public List<WorkoutExerciseDetailDto> Exercises { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsEditable { get; set; }
}

public class WorkoutExerciseDetailDto
{
    public Guid ExerciseId { get; set; }
    public int Order { get; set; }
    public int? Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Intensity { get; set; }
    public int? RestSeconds { get; set; }
}