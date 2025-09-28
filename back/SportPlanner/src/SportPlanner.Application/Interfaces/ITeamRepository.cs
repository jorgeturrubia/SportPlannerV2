using SportPlanner.Domain.Entities;

namespace SportPlanner.Application.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Team>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<List<Team>> GetActiveBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameInSubscriptionAsync(Guid subscriptionId, string name, CancellationToken cancellationToken = default);
    Task<int> CountActiveTeamsBySubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(Team team, CancellationToken cancellationToken = default);
}