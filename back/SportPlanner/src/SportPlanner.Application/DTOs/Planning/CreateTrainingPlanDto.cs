namespace SportPlanner.Application.DTOs.Planning;

public class CreateTrainingPlanDto
{
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TrainingScheduleDto Schedule { get; set; }
    public List<AddObjectiveToPlanDto> Objectives { get; set; } = new();
}

public class AddObjectiveToPlanDto
{
    public Guid ObjectiveId { get; set; }
    public int Priority { get; set; }
    public int TargetSessions { get; set; }
}