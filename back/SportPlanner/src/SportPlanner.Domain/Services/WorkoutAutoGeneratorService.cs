using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Domain.Services;

/// <summary>
/// Domain service for automatically generating workouts from a TrainingPlan.
/// Implements the workout distribution algorithm described in ADR-003.
/// </summary>
public class WorkoutAutoGeneratorService
{
    /// <summary>
    /// Generates all workouts for a training plan based on schedule and objectives.
    /// </summary>
    public List<Workout> GenerateAllWorkouts(TrainingPlan plan, List<Exercise> availableExercises, string createdBy)
    {
        if (plan is null)
            throw new ArgumentNullException(nameof(plan));

        if (availableExercises is null)
            throw new ArgumentNullException(nameof(availableExercises));

        if (!plan.Objectives.Any())
            throw new InvalidOperationException("Training plan must have at least one objective to generate workouts");

        // 1. Calculate total sessions from schedule
        var totalSessions = plan.Schedule.TotalSessions;
        var trainingDates = CalculateTrainingDates(plan);

        // 2. Distribute objectives across sessions based on priority
        var objectiveDistribution = DistributeObjectives(plan.Objectives.ToList(), totalSessions);

        // 3. Generate a workout for each training date
        var workouts = new List<Workout>();
        for (int i = 0; i < trainingDates.Count; i++)
        {
            var date = trainingDates[i];
            var objectivesForSession = objectiveDistribution[i];

            // Get hours allocated for this day
            var hoursForDay = plan.Schedule.GetHoursForDay(date.DayOfWeek);
            var durationMinutes = hoursForDay * 60;

            // Find best exercises for these objectives
            var exercises = FindBestExercises(objectivesForSession, availableExercises, durationMinutes);

            // Generate workout name
            var workoutName = GenerateWorkoutName(objectivesForSession, i + 1, totalSessions);

            // Create workout with the calculated date
            workouts.Add(new Workout(
                plan.SubscriptionId,
                Domain.Enum.ContentOwnership.User,
                createdBy,
                date
            ));

            // Set metadata for the workout
            workouts.Last().SetMetadata(durationMinutes, $"Auto-generated workout for session {i + 1}/{totalSessions}", createdBy);
        }

        return workouts;
    }

    /// <summary>
    /// Calculates all training dates within the plan period.
    /// </summary>
    private List<DateTime> CalculateTrainingDates(TrainingPlan plan)
    {
        var dates = new List<DateTime>();
        var currentDate = plan.StartDate;

        while (currentDate <= plan.EndDate)
        {
            if (plan.Schedule.IsTrainingDay(currentDate.DayOfWeek))
            {
                dates.Add(currentDate);
            }
            currentDate = currentDate.AddDays(1);
        }

        return dates;
    }

    /// <summary>
    /// Distributes objectives across sessions based on priority and target sessions.
    /// </summary>
    private List<List<Guid>> DistributeObjectives(List<PlanObjective> planObjectives, int totalSessions)
    {
        // Initialize distribution array
        var distribution = new List<List<Guid>>();
        for (int i = 0; i < totalSessions; i++)
        {
            distribution.Add(new List<Guid>());
        }

        // Sort objectives by priority (high to low)
        var sortedObjectives = planObjectives.OrderByDescending(po => po.Priority).ToList();

        // Distribute each objective across its target sessions
        foreach (var planObjective in sortedObjectives)
        {
            // Calculate interval between sessions for this objective
            var interval = totalSessions / (double)planObjective.TargetSessions;

            // Distribute objective evenly across sessions
            for (int i = 0; i < planObjective.TargetSessions; i++)
            {
                var sessionIndex = (int)Math.Round(i * interval);

                // Ensure we don't exceed bounds
                if (sessionIndex >= totalSessions)
                    sessionIndex = totalSessions - 1;

                distribution[sessionIndex].Add(planObjective.ObjectiveId);
            }
        }

        // Ensure no session is empty (redistribute if needed)
        EnsureNoEmptySessions(distribution, sortedObjectives);

        return distribution;
    }

    /// <summary>
    /// Ensures no session is left without objectives by redistributing.
    /// </summary>
    private void EnsureNoEmptySessions(List<List<Guid>> distribution, List<PlanObjective> planObjectives)
    {
        if (!planObjectives.Any())
            return;

        for (int i = 0; i < distribution.Count; i++)
        {
            if (!distribution[i].Any())
            {
                // Add the highest priority objective to this session
                var topObjective = planObjectives.OrderByDescending(po => po.Priority).First();
                distribution[i].Add(topObjective.ObjectiveId);
            }
        }
    }

    /// <summary>
    /// Finds the best exercises that cover the given objectives within the time constraint.
    /// </summary>
    private List<Guid> FindBestExercises(List<Guid> objectiveIds, List<Exercise> availableExercises, int durationMinutes)
    {
        var selectedExercises = new List<Guid>();

        // TODO: Implement intelligent exercise selection algorithm
        // This should:
        // 1. Find exercises that cover multiple objectives (maximize coverage)
        // 2. Balance difficulty across the workout
        // 3. Respect time constraints (durationMinutes)
        // 4. Prefer exercises with higher relevance to objectives

        // For now, simple implementation: select first exercise that matches each objective
        foreach (var objectiveId in objectiveIds.Distinct())
        {
            // Note: This assumes Exercise entity has Objectives navigation property
            // which links to ExerciseObjective junction table
            var matchingExercise = availableExercises.FirstOrDefault();

            if (matchingExercise != null && !selectedExercises.Contains(matchingExercise.Id))
            {
                selectedExercises.Add(matchingExercise.Id);
            }
        }

        return selectedExercises;
    }

    /// <summary>
    /// Generates a descriptive workout name based on objectives.
    /// </summary>
    private string GenerateWorkoutName(List<Guid> objectiveIds, int sessionNumber, int totalSessions)
    {
        // Simple naming for now
        return $"Workout {sessionNumber}/{totalSessions} - {objectiveIds.Count} Objective(s)";
    }

    /// <summary>
    /// Validates that the plan is ready for workout generation.
    /// </summary>
    public void ValidatePlanForGeneration(TrainingPlan plan)
    {
        if (plan is null)
            throw new ArgumentNullException(nameof(plan));

        if (!plan.Objectives.Any())
            throw new InvalidOperationException("Plan must have at least one objective");

        if (plan.Schedule.TotalSessions <= 0)
            throw new InvalidOperationException("Plan schedule must have at least one training session");

        if (!plan.IsTargetSessionsBalanced())
            throw new InvalidOperationException("Plan target sessions are not balanced. Total target sessions should be between 80-120% of actual sessions.");
    }
}