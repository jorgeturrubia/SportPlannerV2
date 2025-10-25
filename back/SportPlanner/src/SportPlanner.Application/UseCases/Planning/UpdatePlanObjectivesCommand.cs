using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Command to replace/update ALL objectives for a training plan
/// This removes objectives not in the new list and adds new ones
/// Essentially a complete synchronization of the objectives collection
/// </summary>
public record UpdatePlanObjectivesCommand(
    Guid TrainingPlanId,
    List<AddObjectiveBatchItem> Objectives
) : IRequest<Unit>;
