namespace SportPlanner.Application.DTOs.Planning;

public class CreateWorkoutDto
{
    public DateTime Fecha { get; set; }
    public Guid? ObjectiveId { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public string? Notes { get; set; }
    public List<WorkoutExerciseDto> Exercises { get; set; } = new();
}

public class WorkoutExerciseDto
{
    public Guid ExerciseId { get; set; }
    public int Order { get; set; }
    public int? Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Intensity { get; set; }
    public int? RestSeconds { get; set; }
    public string? Notes { get; set; }
}