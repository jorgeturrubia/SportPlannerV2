using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Individual objective to add to plan in batch operation
/// </summary>
public class AddObjectiveBatchItem
{
    public Guid ObjectiveId { get; set; }
    public int Priority { get; set; } = 3; // 1-5, default 3
    public int TargetSessions { get; set; } = 10; // Default 10
}

/// <summary>
/// Command to add multiple objectives to a training plan in a single transaction
/// </summary>
public record AddMultipleObjectivesToPlanCommand(
    Guid TrainingPlanId,
    List<AddObjectiveBatchItem> Objectives
) : IRequest<Unit>;
