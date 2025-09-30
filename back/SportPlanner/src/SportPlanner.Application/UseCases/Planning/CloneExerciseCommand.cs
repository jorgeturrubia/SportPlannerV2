using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record CloneExerciseCommand(Guid ExerciseId) : IRequest<Guid>;