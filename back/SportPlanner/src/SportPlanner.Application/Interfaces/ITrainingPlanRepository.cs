using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface ITrainingPlanRepository
{
    Task<TrainingPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TrainingPlan?> GetByIdWithObjectivesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TrainingPlan>> GetBySubscriptionIdAsync(Guid subscriptionId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TrainingPlan trainingPlan, CancellationToken cancellationToken = default);
    Task UpdateAsync(TrainingPlan trainingPlan, CancellationToken cancellationToken = default);
    Task DeleteAsync(TrainingPlan trainingPlan, CancellationToken cancellationToken = default);
}