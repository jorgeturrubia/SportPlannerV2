using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ExerciseCategoryRepository : IExerciseCategoryRepository
{
    private readonly SportPlannerDbContext _context;

    public ExerciseCategoryRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<ExerciseCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ExerciseCategories
            .FirstOrDefaultAsync(ec => ec.Id == id, cancellationToken);
    }

    public async Task<List<ExerciseCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ExerciseCategories
            .Where(ec => ec.IsActive)
            .OrderBy(ec => ec.Sport)
            .ThenBy(ec => ec.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ExerciseCategories.AnyAsync(ec => ec.Id == id, cancellationToken);
    }
}