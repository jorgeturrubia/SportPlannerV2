namespace SportPlanner.Domain.ValueObjects;

/// <summary>
/// Value object representing a training schedule with days and hours per week.
/// Immutable object that calculates total sessions and hours.
/// </summary>
public class TrainingSchedule
{
    public DayOfWeek[] TrainingDays { get; private set; }
    public Dictionary<DayOfWeek, int> HoursPerDay { get; private set; }

    // Calculated properties
    public int TotalWeeks { get; private set; }
    public int TotalSessions => TrainingDays.Length * TotalWeeks;
    public int TotalHours => HoursPerDay.Values.Sum() * TotalWeeks;

    // For EF Core
    private TrainingSchedule()
    {
        TrainingDays = Array.Empty<DayOfWeek>();
        HoursPerDay = new Dictionary<DayOfWeek, int>();
    }

    public TrainingSchedule(DayOfWeek[] trainingDays, Dictionary<DayOfWeek, int> hoursPerDay, int totalWeeks)
    {
        if (trainingDays == null || trainingDays.Length == 0)
            throw new ArgumentException("Training days cannot be empty", nameof(trainingDays));

        if (hoursPerDay == null || hoursPerDay.Count == 0)
            throw new ArgumentException("Hours per day cannot be empty", nameof(hoursPerDay));

        if (totalWeeks <= 0)
            throw new ArgumentException("Total weeks must be greater than zero", nameof(totalWeeks));

        // Validate that all training days have hours assigned
        foreach (var day in trainingDays)
        {
            if (!hoursPerDay.ContainsKey(day))
                throw new ArgumentException($"Hours not specified for training day {day}", nameof(hoursPerDay));

            if (hoursPerDay[day] <= 0)
                throw new ArgumentException($"Hours for {day} must be greater than zero", nameof(hoursPerDay));
        }

        // Validate no extra days in hoursPerDay
        foreach (var day in hoursPerDay.Keys)
        {
            if (!trainingDays.Contains(day))
                throw new ArgumentException($"Day {day} is in hours dictionary but not in training days", nameof(hoursPerDay));
        }

        TrainingDays = trainingDays.OrderBy(d => d).ToArray();
        HoursPerDay = new Dictionary<DayOfWeek, int>(hoursPerDay);
        TotalWeeks = totalWeeks;
    }

    /// <summary>
    /// Gets hours for a specific day of the week.
    /// </summary>
    public int GetHoursForDay(DayOfWeek day)
    {
        return HoursPerDay.TryGetValue(day, out var hours) ? hours : 0;
    }

    /// <summary>
    /// Checks if a specific day is a training day.
    /// </summary>
    public bool IsTrainingDay(DayOfWeek day)
    {
        return TrainingDays.Contains(day);
    }

    /// <summary>
    /// Gets average hours per training session.
    /// </summary>
    public double GetAverageHoursPerSession()
    {
        if (TrainingDays.Length == 0) return 0;
        return (double)HoursPerDay.Values.Sum() / TrainingDays.Length;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TrainingSchedule other)
            return false;

        return TrainingDays.SequenceEqual(other.TrainingDays) &&
               HoursPerDay.Count == other.HoursPerDay.Count &&
               HoursPerDay.All(kvp => other.HoursPerDay.TryGetValue(kvp.Key, out var value) && value == kvp.Value) &&
               TotalWeeks == other.TotalWeeks;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            TrainingDays.Aggregate(0, (acc, day) => acc ^ day.GetHashCode()),
            HoursPerDay.Aggregate(0, (acc, kvp) => acc ^ kvp.Key.GetHashCode() ^ kvp.Value.GetHashCode()),
            TotalWeeks);
    }
}