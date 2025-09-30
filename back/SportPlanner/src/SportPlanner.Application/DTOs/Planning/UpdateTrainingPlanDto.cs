namespace SportPlanner.Application.DTOs.Planning;

public class UpdateTrainingPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TrainingScheduleDto Schedule { get; set; }
}