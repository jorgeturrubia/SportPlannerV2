using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.Interfaces;

public interface ISubscriptionUserRepository
{
    Task<int> GetActiveUserCountBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<SubscriptionUser?> GetBySubscriptionAndUserIdAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsUserInSubscriptionAsync(Guid subscriptionId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(SubscriptionUser subscriptionUser, CancellationToken cancellationToken = default);
    Task UpdateAsync(SubscriptionUser subscriptionUser, CancellationToken cancellationToken = default);
    Task<List<SubscriptionUser>> GetActiveUsersBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
}
