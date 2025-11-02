namespace SportPlanner.Application.DTOs.Planning;

public class UpdateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Guid TypeId { get; set; }
    public string? VideoUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? Instructions { get; set; }
    public int? DefaultSets { get; set; }
    public int? DefaultReps { get; set; }
    public int? DefaultDurationSeconds { get; set; }
    public string? DefaultIntensity { get; set; }
    public string? AnimationJson { get; set; }
    public List<Guid> ObjectiveIds { get; set; } = new();
}