using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class TrainingPlanRepository : ITrainingPlanRepository
{
    private readonly SportPlannerDbContext _context;

    public TrainingPlanRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<TrainingPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingPlans
            .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken);
    }

    public async Task<TrainingPlan?> GetByIdWithObjectivesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingPlans
            .Include(tp => tp.Objectives)
                .ThenInclude(po => po.Objective)
                    .ThenInclude(o => o.Category)
            .Include(tp => tp.Objectives)
                .ThenInclude(po => po.Objective)
                    .ThenInclude(o => o.Subcategory)
            .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken);
    }

    public async Task<List<TrainingPlan>> GetBySubscriptionIdAsync(
        Guid subscriptionId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TrainingPlans
            .Include(tp => tp.Objectives)
                .ThenInclude(po => po.Objective)
            .Where(tp => tp.SubscriptionId == subscriptionId);

        if (!includeInactive)
        {
            query = query.Where(tp => tp.IsActive);
        }

        return await query
            .OrderByDescending(tp => tp.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingPlans
            .AnyAsync(tp => tp.Id == id, cancellationToken);
    }

    public async Task AddAsync(TrainingPlan trainingPlan, CancellationToken cancellationToken = default)
    {
        await _context.TrainingPlans.AddAsync(trainingPlan, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TrainingPlan trainingPlan, CancellationToken cancellationToken = default)
    {
        _context.TrainingPlans.Update(trainingPlan);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TrainingPlan trainingPlan, CancellationToken cancellationToken = default)
    {
        _context.TrainingPlans.Remove(trainingPlan);
        await _context.SaveChangesAsync(cancellationToken);
    }
}