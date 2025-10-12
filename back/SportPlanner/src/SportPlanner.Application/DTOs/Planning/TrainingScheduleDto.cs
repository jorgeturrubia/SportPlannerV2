namespace SportPlanner.Application.DTOs.Planning;

public class TrainingScheduleDto
{
    /// <summary>
    /// Days of the week for training (0=Sunday, 1=Monday, ..., 6=Saturday)
    /// </summary>
    public int[] TrainingDays { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Hours per day (key: day of week as int, value: hours)
    /// </summary>
    public Dictionary<int, int> HoursPerDay { get; set; } = new();

    public int TotalWeeks { get; set; }
    public int TotalSessions { get; set; }
    public int TotalHours { get; set; }
}