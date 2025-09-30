using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ObjectiveRepository : IObjectiveRepository
{
    private readonly SportPlannerDbContext _context;

    public ObjectiveRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Objective?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Objectives
            .Include(o => o.Category)
            .Include(o => o.Subcategory)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<List<Objective>> GetBySubscriptionIdAsync(
        Guid subscriptionId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Objectives
            .Include(o => o.Category)
            .Include(o => o.Subcategory)
            .Where(o => o.SubscriptionId == subscriptionId);

        if (!includeInactive)
        {
            query = query.Where(o => o.IsActive);
        }

        return await query
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Objective>> GetSystemObjectivesBySportAsync(
        Sport sport,
        CancellationToken cancellationToken = default)
    {
        return await _context.Objectives
            .Include(o => o.Category)
            .Include(o => o.Subcategory)
            .Where(o => o.Ownership == ContentOwnership.System && o.Sport == sport && o.IsActive)
            .OrderBy(o => o.Category.Name)
            .ThenBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Objectives
            .AnyAsync(o => o.Id == id, cancellationToken);
    }

    public async Task AddAsync(Objective objective, CancellationToken cancellationToken = default)
    {
        await _context.Objectives.AddAsync(objective, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Objective objective, CancellationToken cancellationToken = default)
    {
        _context.Objectives.Update(objective);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Objective objective, CancellationToken cancellationToken = default)
    {
        _context.Objectives.Remove(objective);
        await _context.SaveChangesAsync(cancellationToken);
    }
}