using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record CreateExerciseCommand(CreateExerciseDto Exercise) : IRequest<Guid>;