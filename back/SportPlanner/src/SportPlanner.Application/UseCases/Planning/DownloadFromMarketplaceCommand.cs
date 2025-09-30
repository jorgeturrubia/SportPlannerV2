using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record DownloadFromMarketplaceCommand(Guid MarketplaceItemId) : IRequest<Guid>;