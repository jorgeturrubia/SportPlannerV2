namespace SportPlanner.Application.DTOs.Planning;

public class PlanObjectiveDto
{
    public Guid ObjectiveId { get; set; }
    public int Priority { get; set; }
    public int TargetSessions { get; set; }

    // Include objective details for convenience
    public string? ObjectiveName { get; set; }
    public string? ObjectiveDescription { get; set; }
    // Objective level (1-5)
    public int Level { get; set; } = 1;
}