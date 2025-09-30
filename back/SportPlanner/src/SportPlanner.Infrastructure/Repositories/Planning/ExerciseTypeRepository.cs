using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ExerciseTypeRepository : IExerciseTypeRepository
{
    private readonly SportPlannerDbContext _context;

    public ExerciseTypeRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<ExerciseType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ExerciseTypes
            .FirstOrDefaultAsync(et => et.Id == id, cancellationToken);
    }

    public async Task<List<ExerciseType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ExerciseTypes
            .Where(et => et.IsActive)
            .OrderBy(et => et.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ExerciseTypes.AnyAsync(et => et.Id == id, cancellationToken);
    }
}