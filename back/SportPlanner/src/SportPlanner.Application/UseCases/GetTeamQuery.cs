using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record GetTeamQuery(
    Guid SubscriptionId,
    Guid TeamId
) : IRequest<TeamResponse?>;
