using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly SportPlannerDbContext _context;

    public TeamRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Team?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Category)
            .Include(t => t.Gender)
            .Include(t => t.AgeGroup)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Team>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Category)
            .Include(t => t.Gender)
            .Include(t => t.AgeGroup)
            .Where(t => t.SubscriptionId == subscriptionId)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Team>> GetActiveBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Category)
            .Include(t => t.Gender)
            .Include(t => t.AgeGroup)
            .Where(t => t.SubscriptionId == subscriptionId && t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithNameInSubscriptionAsync(Guid subscriptionId, string name, CancellationToken cancellationToken = default, Guid? excludeTeamId = null)
    {
        var query = _context.Teams
            .Where(t => t.SubscriptionId == subscriptionId && t.Name == name);

        if (excludeTeamId.HasValue)
        {
            query = query.Where(t => t.Id != excludeTeamId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<int> CountActiveTeamsBySubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .CountAsync(t => t.SubscriptionId == subscriptionId && t.IsActive, cancellationToken);
    }

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Remove(team);
        await _context.SaveChangesAsync(cancellationToken);
    }
}