using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class MarketplaceItemRepository : IMarketplaceItemRepository
{
    private readonly SportPlannerDbContext _context;

    public MarketplaceItemRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<MarketplaceItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MarketplaceItems
            .Include(mi => mi.Ratings)
            .FirstOrDefaultAsync(mi => mi.Id == id, cancellationToken);
    }

    public async Task<List<MarketplaceItem>> SearchAsync(
        Sport sport,
        MarketplaceItemType? type,
        MarketplaceFilter filter,
        string? searchTerm,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(sport, type, filter, searchTerm);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Sport sport,
        MarketplaceItemType? type,
        MarketplaceFilter filter,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(sport, type, filter, searchTerm);
        return await query.CountAsync(cancellationToken);
    }

    public async Task AddAsync(MarketplaceItem item, CancellationToken cancellationToken = default)
    {
        await _context.MarketplaceItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MarketplaceItem item, CancellationToken cancellationToken = default)
    {
        _context.MarketplaceItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<MarketplaceItem> BuildSearchQuery(
        Sport sport,
        MarketplaceItemType? type,
        MarketplaceFilter filter,
        string? searchTerm)
    {
        var query = _context.MarketplaceItems
            .Where(mi => mi.Sport == sport);

        if (type.HasValue)
        {
            query = query.Where(mi => mi.Type == type.Value);
        }

        switch (filter)
        {
            case MarketplaceFilter.OfficialOnly:
                query = query.Where(mi => mi.IsSystemOfficial);
                break;
            case MarketplaceFilter.CommunityOnly:
                query = query.Where(mi => !mi.IsSystemOfficial);
                break;
            case MarketplaceFilter.Popular:
                query = query.OrderByDescending(mi => mi.TotalDownloads);
                break;
            case MarketplaceFilter.TopRated:
                query = query.OrderByDescending(mi => mi.AverageRating)
                             .ThenByDescending(mi => mi.TotalRatings);
                break;
            case MarketplaceFilter.All:
            default:
                query = query.OrderByDescending(mi => mi.PublishedAt);
                break;
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(mi =>
                mi.Name.ToLower().Contains(searchLower) ||
                mi.Description.ToLower().Contains(searchLower));
        }

        return query;
    }
}