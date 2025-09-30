using MediatR;
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

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Create training schedule value object
        var schedule = new TrainingSchedule(
            dto.Schedule.TrainingDays,
            dto.Schedule.HoursPerDay,
            dto.Schedule.TotalWeeks);

        // Create training plan
        var trainingPlan = new TrainingPlan(
            subscription.Id,
            dto.Name,
            dto.StartDate,
            dto.EndDate,
            schedule);

        // Add objectives
        foreach (var objectiveDto in dto.Objectives)
        {
            // Validate objective exists
            if (!await _objectiveRepository.ExistsAsync(objectiveDto.ObjectiveId, cancellationToken))
            {
                throw new InvalidOperationException($"Objective with ID {objectiveDto.ObjectiveId} does not exist");
            }

            trainingPlan.AddObjective(objectiveDto.ObjectiveId, objectiveDto.Priority, objectiveDto.TargetSessions);
        }

        await _trainingPlanRepository.AddAsync(trainingPlan, cancellationToken);

        return trainingPlan.Id;
    }
}