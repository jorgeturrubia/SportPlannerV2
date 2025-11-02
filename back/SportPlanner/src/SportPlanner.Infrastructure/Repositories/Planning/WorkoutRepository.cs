using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly SportPlannerDbContext _context;

    public WorkoutRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Workout?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Workouts
            .Include(w => w.Exercises)
                .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<List<Workout>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Workouts
            .Include(w => w.Exercises)
            .Where(w => w.SubscriptionId == subscriptionId && w.IsActive)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WorkoutDto>> GetWorkoutsDtoBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Workouts
            .Where(w => w.SubscriptionId == subscriptionId && w.IsActive)
            .Include(w => w.Exercises)
                .ThenInclude(we => we.Exercise)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkoutDto
            {
                Id = w.Id,
                SubscriptionId = w.SubscriptionId,
                Ownership = w.Ownership,
                Fecha = w.Fecha,
                ObjectiveId = null, // Workouts don't have direct objective relationship
                ObjectiveName = null, // Workouts don't have direct objective relationship
                EstimatedDurationMinutes = w.EstimatedDurationMinutes,
                Notes = w.Notes,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt,
                IsEditable = w.Ownership == Domain.Enum.ContentOwnership.User,
                Exercises = w.Exercises.Select(we => new WorkoutExerciseDetailDto
                {
                    ExerciseId = we.ExerciseId,
                    Order = we.Order,
                    Sets = we.Sets,
                    Reps = we.Reps,
                    DurationSeconds = we.DurationSeconds,
                    Intensity = we.Intensity,
                    RestSeconds = we.RestSeconds
                }).OrderBy(we => we.Order).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Workout>> GetByPlanIdAsync(Guid trainingPlanId, CancellationToken cancellationToken = default)
    {
        // Note: This assumes Workout has a TrainingPlanId property
        // If not, this method may need adjustment based on actual domain model
        return await _context.Workouts
            .Include(w => w.Exercises)
            .Where(w => w.IsActive)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        await _context.Workouts.AddAsync(workout, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workout = await GetByIdAsync(id, cancellationToken);
        if (workout != null)
        {
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}