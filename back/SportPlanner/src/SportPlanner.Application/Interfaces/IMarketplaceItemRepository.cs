using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.Interfaces;

public interface IMarketplaceItemRepository
{
    Task<MarketplaceItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<MarketplaceItem>> SearchAsync(
        Sport sport,
        MarketplaceItemType? type,
        MarketplaceFilter filter,
        string? searchTerm,
        int skip,
        int take,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Sport sport,
        MarketplaceItemType? type,
        MarketplaceFilter filter,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task AddAsync(MarketplaceItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(MarketplaceItem item, CancellationToken cancellationToken = default);
}