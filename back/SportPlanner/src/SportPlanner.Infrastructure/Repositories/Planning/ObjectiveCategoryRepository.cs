using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;
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
            .FirstOrDefaultAsync(oc => oc.Id == id, cancellationToken);
    }

    public async Task<List<ObjectiveCategory>> GetBySportAsync(Sport sport, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .Where(oc => oc.Sport == sport)
            .OrderBy(oc => oc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ObjectiveCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .OrderBy(oc => oc.Sport)
            .ThenBy(oc => oc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveCategories
            .AnyAsync(oc => oc.Id == id, cancellationToken);
    }
}