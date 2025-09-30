using MediatR;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.UseCases.Planning;

public record PublishToMarketplaceCommand(
    MarketplaceItemType Type,
    Guid SourceEntityId) : IRequest<Guid>;