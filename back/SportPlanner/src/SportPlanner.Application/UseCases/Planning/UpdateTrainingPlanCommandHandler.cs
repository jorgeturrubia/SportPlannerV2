using MediatR;
using SportPlanner.Application.Common;
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

        // 🔍 LOGGING: Debug incoming data
        Console.WriteLine($"🔍 UpdateTrainingPlanCommandHandler - DTO received:");
        Console.WriteLine($"🔍 Id: {dto.Id}");
        Console.WriteLine($"🔍 Name: {dto.Name}");
        Console.WriteLine($"🔍 StartDate: {dto.StartDate} (Type: {dto.StartDate.GetType()})");
        Console.WriteLine($"🔍 EndDate: {dto.EndDate} (Type: {dto.EndDate.GetType()})");
        Console.WriteLine($"🔍 StartDate.Kind: {dto.StartDate.Kind}");
        Console.WriteLine($"🔍 EndDate.Kind: {dto.EndDate.Kind}");

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

        // Convert int[] to DayOfWeek[] and Dictionary<int, int> to Dictionary<DayOfWeek, int>
        var trainingDays = dto.Schedule.TrainingDays
            .Select(d => (DayOfWeek)d)
            .ToArray();

        var hoursPerDay = dto.Schedule.HoursPerDay
            .ToDictionary(kvp => (DayOfWeek)kvp.Key, kvp => kvp.Value);

        // Create training schedule value object
        var schedule = new TrainingSchedule(
            trainingDays,
            hoursPerDay,
            dto.Schedule.TotalWeeks);

        // Ensure dates are UTC (Postgres timestamptz requires UTC or DateTimeOffset)

        var startDateUtc = dto.StartDate.ToUtcKind();
        var endDateUtc = dto.EndDate.ToUtcKind();


        // Update training plan
        trainingPlan.Update(dto.Name, startDateUtc, endDateUtc, schedule);

        await _trainingPlanRepository.UpdateAsync(trainingPlan, cancellationToken);

        return Unit.Value;
    }
}