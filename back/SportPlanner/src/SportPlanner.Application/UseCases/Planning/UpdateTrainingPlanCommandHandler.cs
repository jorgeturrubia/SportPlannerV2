using MediatR;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.ValueObjects;

namespace SportPlanner.Application.UseCases.Planning;

public class UpdateTrainingPlanCommandHandler : IRequestHandler<UpdateTrainingPlanCommand, Unit>
{
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTrainingPlanCommandHandler(
        ITrainingPlanRepository trainingPlanRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _trainingPlanRepository = trainingPlanRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateTrainingPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();
        var dto = request.TrainingPlan;

        // Get user's subscription
        var subscription = await _subscriptionRepository.GetByOwnerIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User does not have an active subscription");

        // Get training plan
        var trainingPlan = await _trainingPlanRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Training plan with ID {dto.Id} not found");

        // Verify ownership
        if (trainingPlan.SubscriptionId != subscription.Id)
        {
            throw new UnauthorizedAccessException("Cannot modify training plan from another subscription");
        }

        // Create training schedule value object
        var schedule = new TrainingSchedule(
            dto.Schedule.TrainingDays,
            dto.Schedule.HoursPerDay,
            dto.Schedule.TotalWeeks);

        // Update training plan
        trainingPlan.Update(dto.Name, dto.StartDate, dto.EndDate, schedule);

        await _trainingPlanRepository.UpdateAsync(trainingPlan, cancellationToken);

        return Unit.Value;
    }
}