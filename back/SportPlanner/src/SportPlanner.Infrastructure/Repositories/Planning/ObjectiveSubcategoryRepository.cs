using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ObjectiveSubcategoryRepository : IObjectiveSubcategoryRepository
{
    private readonly SportPlannerDbContext _context;

    public ObjectiveSubcategoryRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<ObjectiveSubcategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveSubcategories
            .Include(osc => osc.Category)
            .FirstOrDefaultAsync(osc => osc.Id == id, cancellationToken);
    }

    public async Task<List<ObjectiveSubcategory>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveSubcategories
            .Where(osc => osc.ObjectiveCategoryId == categoryId)
            .OrderBy(osc => osc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ObjectiveSubcategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveSubcategories
            .Include(osc => osc.Category)
            .OrderBy(osc => osc.Category.Sport)
            .ThenBy(osc => osc.Category.Name)
            .ThenBy(osc => osc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ObjectiveSubcategories
            .AnyAsync(osc => osc.Id == id, cancellationToken);
    }
}