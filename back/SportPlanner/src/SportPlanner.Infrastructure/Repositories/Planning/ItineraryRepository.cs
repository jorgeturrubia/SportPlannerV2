using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ItineraryRepository : IItineraryRepository
{
    private readonly SportPlannerDbContext _context;

    public ItineraryRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Itinerary?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Itineraries.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Itinerary>> GetAllWithItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Itineraries
            .Include(i => i.Items)
                .ThenInclude(imi => imi.MarketplaceItem)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Itinerary itinerary, CancellationToken cancellationToken = default)
    {
        await _context.Itineraries.AddAsync(itinerary, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Itinerary itinerary, CancellationToken cancellationToken = default)
    {
        _context.Itineraries.Update(itinerary);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var itinerary = await GetByIdAsync(id, cancellationToken);
        if (itinerary != null)
        {
            _context.Itineraries.Remove(itinerary);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}