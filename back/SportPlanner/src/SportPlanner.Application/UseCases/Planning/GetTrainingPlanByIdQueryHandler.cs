using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetTrainingPlanByIdQueryHandler : IRequestHandler<GetTrainingPlanByIdQuery, TrainingPlanDto>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTrainingPlanByIdQueryHandler(
        ITrainingPlanRepository trainingPlanRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TrainingPlanDto> Handle(GetTrainingPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get training plan
        var tp = await _trainingPlanRepository.GetByIdWithObjectivesAsync(request.TrainingPlanId, cancellationToken)
            ?? throw new InvalidOperationException($"Training plan with ID {request.TrainingPlanId} not found");

        // Verify ownership
        if (tp.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot access training plan from another subscription");
        }

        // Map to DTO
        return new TrainingPlanDto
        {
            Id = tp.Id,
            SubscriptionId = tp.SubscriptionId,
            Name = tp.Name,
            StartDate = tp.StartDate,
            EndDate = tp.EndDate,
            Schedule = new TrainingScheduleDto
            {
                TrainingDays = tp.Schedule.TrainingDays,
                HoursPerDay = tp.Schedule.HoursPerDay,
                TotalWeeks = tp.Schedule.TotalWeeks,
                TotalSessions = tp.Schedule.TotalSessions,
                TotalHours = tp.Schedule.TotalHours
            },
            IsActive = tp.IsActive,
            MarketplaceItemId = tp.MarketplaceItemId,
            Objectives = tp.Objectives.Select(po => new PlanObjectiveDto
            {
                ObjectiveId = po.ObjectiveId,
                Priority = po.Priority,
                TargetSessions = po.TargetSessions,
                ObjectiveName = po.Objective?.Name ?? "",
                ObjectiveDescription = po.Objective?.Description ?? ""
            }).ToList(),
            CreatedAt = tp.CreatedAt,
            UpdatedAt = tp.UpdatedAt,
            IsTargetSessionsBalanced = tp.IsTargetSessionsBalanced()
        };
    }
}