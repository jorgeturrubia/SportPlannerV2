using MediatR;
using SportPlanner.Application.DTOs;

namespace SportPlanner.Application.UseCases;

public record GetTeamsQuery(
    Guid SubscriptionId,
    bool ActiveOnly = true
) : IRequest<List<TeamResponse>>;
