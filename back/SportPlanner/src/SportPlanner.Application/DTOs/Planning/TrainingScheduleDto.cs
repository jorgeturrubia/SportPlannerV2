namespace SportPlanner.Application.DTOs.Planning;

public class TrainingScheduleDto
{
    public DayOfWeek[] TrainingDays { get; set; } = Array.Empty<DayOfWeek>();
    public Dictionary<DayOfWeek, int> HoursPerDay { get; set; } = new();
    public int TotalWeeks { get; set; }
    public int TotalSessions { get; set; }
    public int TotalHours { get; set; }
}