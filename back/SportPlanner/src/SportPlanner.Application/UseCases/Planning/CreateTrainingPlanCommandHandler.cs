using MediatR;
using SportPlanner.Application.Common;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Application.UseCases.Planning;

public class CreateTrainingPlanCommandHandler : IRequestHandler<CreateTrainingPlanCommand, Guid>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateTrainingPlanCommandHandler(
        ITrainingPlanRepository trainingPlanRepository,
        IObjectiveRepository objectiveRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _objectiveRepository = objectiveRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateTrainingPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.TrainingPlan;

        // 🔍 LOGGING: Debug incoming data
        Console.WriteLine($"🔍 CreateTrainingPlanCommandHandler - DTO received:");
        Console.WriteLine($"🔍 Name: {dto.Name}");
        Console.WriteLine($"🔍 StartDate: {dto.StartDate} (Type: {dto.StartDate.GetType()})");
        Console.WriteLine($"🔍 EndDate: {dto.EndDate} (Type: {dto.EndDate.GetType()})");
        Console.WriteLine($"🔍 StartDate.Kind: {dto.StartDate.Kind}");
        Console.WriteLine($"🔍 EndDate.Kind: {dto.EndDate.Kind}");

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Convert int[] to DayOfWeek[] and Dictionary<int, int> to Dictionary<DayOfWeek, int>
        var trainingDays = dto.Schedule?.TrainingDays
            ?.Select(d => (DayOfWeek)d)
            .ToArray() ?? Array.Empty<DayOfWeek>();

        var hoursPerDay = dto.Schedule?.HoursPerDay
            ?.ToDictionary(kvp => (DayOfWeek)kvp.Key, kvp => kvp.Value) ?? new Dictionary<DayOfWeek, int>();

        // Create training schedule value object
        var schedule = new TrainingSchedule(
            trainingDays,
            hoursPerDay,
            dto.Schedule?.TotalWeeks ?? 0);

        // Ensure dates are UTC (Postgres timestamptz requires UTC or DateTimeOffset)

        var startDateUtc = dto.StartDate.ToUtcKind();
        var endDateUtc = dto.EndDate.ToUtcKind();

        Console.WriteLine($"🔍 After ToUtcKind conversion:");
        Console.WriteLine($"🔍 startDateUtc: {startDateUtc} (Kind: {startDateUtc.Kind})");
        Console.WriteLine($"🔍 endDateUtc: {endDateUtc} (Kind: {endDateUtc.Kind})");

        // Create training plan
        var trainingPlan = new TrainingPlan(
            subscription.Id,
            dto.Name,
            startDateUtc,
            endDateUtc,
            schedule);

        // Set audit fields
        trainingPlan.CreatedBy = userId.ToString();

        // Set IsActive if specified as false (default is true)
        if (!dto.IsActive)
        {
            trainingPlan.Deactivate();
        }

        // Add objectives
        if (dto.Objectives != null)
        {
            foreach (var objectiveDto in dto.Objectives)
            {
                // Validate objective exists
                if (!await _objectiveRepository.ExistsAsync(objectiveDto.ObjectiveId, cancellationToken))
                {
                    throw new InvalidOperationException($"Objective with ID {objectiveDto.ObjectiveId} does not exist");
                }

                trainingPlan.AddObjective(objectiveDto.ObjectiveId, objectiveDto.Priority, objectiveDto.TargetSessions);
            }
        }
        try
        {
        await _trainingPlanRepository.AddAsync(trainingPlan, cancellationToken);

        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return trainingPlan.Id;
    }
}