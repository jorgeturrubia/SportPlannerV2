using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.DTOs.Planning;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class ExerciseRepository : IExerciseRepository
{
    private readonly SportPlannerDbContext _context;

    public ExerciseRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<Exercise>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Where(e => e.SubscriptionId == subscriptionId && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ExerciseDto>> GetExercisesDtoBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Where(e => e.SubscriptionId == subscriptionId && e.IsActive)
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseDto
            {
                Id = e.Id,
                SubscriptionId = e.SubscriptionId,
                Ownership = e.Ownership,
                Name = e.Name,
                Description = e.Description,
                Instructions = e.Instructions,
                AnimationJson = e.AnimationJson,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt,
                // Include related objectives
                ObjectiveIds = _context.ExerciseObjectives
                    .Where(eo => eo.ExerciseId == e.Id)
                    .Select(eo => eo.ObjectiveId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Exercise>> GetSystemExercisesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Where(e => e.SubscriptionId == null && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        await _context.Exercises.AddAsync(exercise, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        _context.Exercises.Update(exercise);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exercise = await GetByIdAsync(id, cancellationToken);
        if (exercise != null)
        {
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateExerciseObjectivesAsync(Guid exerciseId, List<Guid> objectiveIds, CancellationToken cancellationToken = default)
    {
        // Remove existing relationships
        var existingRelationships = await _context.ExerciseObjectives
            .Where(eo => eo.ExerciseId == exerciseId)
            .ToListAsync(cancellationToken);

        _context.ExerciseObjectives.RemoveRange(existingRelationships);

        // Add new relationships
        foreach (var objectiveId in objectiveIds)
        {
            var exerciseObjective = new ExerciseObjective(exerciseId, objectiveId);
            await _context.ExerciseObjectives.AddAsync(exerciseObjective, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}