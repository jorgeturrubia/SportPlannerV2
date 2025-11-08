using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ObjectiveCategoryRepository : IObjectiveCategoryRepository
{
    private readonly SportPlannerDbContext _context;

    public ObjectiveCategoryRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<ObjectiveCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .Include(oc => oc.Sport)
            .FirstOrDefaultAsync(oc => oc.Id == id, cancellationToken);
    }

    public async Task<List<ObjectiveCategory>> GetBySportAsync(Guid sportId, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .Include(oc => oc.Sport)
            .Where(oc => oc.SportId == sportId)
            .OrderBy(oc => oc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ObjectiveCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .Include(oc => oc.Sport)
            .OrderBy(oc => oc.Sport.Name)
            .ThenBy(oc => oc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .AnyAsync(oc => oc.Id == id, cancellationToken);
    }

    public async Task AddAsync(ObjectiveCategory category, CancellationToken cancellationToken = default)
    {
        await _context.ObjectiveCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ObjectiveCategory category, CancellationToken cancellationToken = default)
    {
        _context.ObjectiveCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            _context.ObjectiveCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}