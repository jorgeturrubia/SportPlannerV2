using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);
}
