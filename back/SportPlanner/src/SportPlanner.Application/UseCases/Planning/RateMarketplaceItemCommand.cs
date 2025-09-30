using MediatR;

namespace SportPlanner.Application.UseCases.Planning;

public record RateMarketplaceItemCommand(
    Guid MarketplaceItemId,
    int Stars,
    string? Comment) : IRequest<Unit>;