using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record CreateObjectiveCommand(CreateObjectiveDto Objective) : IRequest<Guid>;