using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record UpdateExerciseCommand(Guid ExerciseId, UpdateExerciseDto Exercise) : IRequest<Unit>;