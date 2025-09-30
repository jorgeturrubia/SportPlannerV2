using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Domain.Enum;

namespace SportPlanner.Application.Interfaces;

public interface IObjectiveRepository
{
    Task<Objective?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Objective>> GetBySubscriptionIdAsync(Guid subscriptionId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<List<Objective>> GetSystemObjectivesBySportAsync(Sport sport, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Objective objective, CancellationToken cancellationToken = default);
    Task UpdateAsync(Objective objective, CancellationToken cancellationToken = default);
    Task DeleteAsync(Objective objective, CancellationToken cancellationToken = default);
}