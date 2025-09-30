using MediatR;
using SportPlanner.Application.DTOs.Planning;

namespace SportPlanner.Application.UseCases.Planning;

public record GetObjectivesBySubscriptionQuery(bool IncludeInactive = false) : IRequest<List<ObjectiveDto>>;