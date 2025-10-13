using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;

namespace SportPlanner.Application.UseCases.Planning;

public class GetTrainingPlansBySubscriptionQueryHandler : IRequestHandler<GetTrainingPlansBySubscriptionQuery, List<TrainingPlanDto>>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTrainingPlansBySubscriptionQueryHandler(
        ITrainingPlanRepository trainingPlanRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<TrainingPlanDto>> Handle(GetTrainingPlansBySubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get training plans
        var trainingPlans = await _trainingPlanRepository.GetBySubscriptionIdAsync(
            subscription.Id,
            request.IncludeInactive,
            cancellationToken);

        // Map to DTOs
        return trainingPlans.Select(tp => new TrainingPlanDto
        {
            Id = tp.Id,
            SubscriptionId = tp.SubscriptionId,
            Name = tp.Name,
            StartDate = tp.StartDate,
            EndDate = tp.EndDate,
            Schedule = new TrainingScheduleDto
            {
                TrainingDays = tp.Schedule.TrainingDays.Select(d => (int)d).ToArray(),
                HoursPerDay = tp.Schedule.HoursPerDay.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value),
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
                ObjectiveDescription = po.Objective?.Description ?? "",
                Level = po.Objective?.Level ?? 1
            }).ToList(),
            CreatedAt = tp.CreatedAt,
            UpdatedAt = tp.UpdatedAt,
            IsTargetSessionsBalanced = tp.IsTargetSessionsBalanced()
        }).ToList();
    }
}