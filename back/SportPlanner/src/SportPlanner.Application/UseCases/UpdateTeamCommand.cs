using MediatR;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases;

public record UpdateTeamCommand(
    Guid SubscriptionId,
    Guid TeamId,
    string Name,
    TeamColor Color,
    string? Description = null
) : IRequest<bool>;
