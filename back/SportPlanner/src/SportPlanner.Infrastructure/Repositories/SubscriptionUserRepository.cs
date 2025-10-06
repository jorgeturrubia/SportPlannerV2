using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SportPlanner.Infrastructure.Repositories;

public class SubscriptionUserRepository : ISubscriptionUserRepository
{
    private readonly SportPlannerDbContext _context;

    public SubscriptionUserRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionUsers
            .FirstOrDefaultAsync(su => su.Id == id, cancellationToken);
    }

    public async Task<int> GetActiveUserCountBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionUsers
            .Where(su => su.SubscriptionId == subscriptionId && su.RemovedAt == null)
            .CountAsync(cancellationToken);
    }

    public async Task<SubscriptionUser?> GetBySubscriptionAndUserIdAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionUsers
            .FirstOrDefaultAsync(su => su.SubscriptionId == subscriptionId && su.UserId == userId && su.RemovedAt == null, cancellationToken);
    }

    public async Task<bool> ExistsUserInSubscriptionAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionUsers
            .AnyAsync(su => su.SubscriptionId == subscriptionId && su.UserId == userId && su.RemovedAt == null, cancellationToken);
    }

    public async Task AddAsync(SubscriptionUser subscriptionUser, CancellationToken cancellationToken = default)
    {
        await _context.SubscriptionUsers.AddAsync(subscriptionUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SubscriptionUser subscriptionUser, CancellationToken cancellationToken = default)
    {
        _context.SubscriptionUsers.Update(subscriptionUser);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SubscriptionUser>> GetActiveUsersBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.SubscriptionUsers
            .Where(su => su.SubscriptionId == subscriptionId && su.RemovedAt == null)
            .ToListAsync(cancellationToken);
    }
}
