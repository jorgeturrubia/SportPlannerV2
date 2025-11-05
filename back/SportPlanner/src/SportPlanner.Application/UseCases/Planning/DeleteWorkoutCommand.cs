using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

/// <summary>
/// Command to delete a workout
/// </summary>
public sealed record DeleteWorkoutCommand(Guid WorkoutId) : IRequest<Unit>;
