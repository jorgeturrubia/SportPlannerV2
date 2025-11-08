using MediatR;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Domain.Entities;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public record GetSystemObjectivesBySportQuery(Sport Sport) : IRequest<List<ObjectiveDto>>;