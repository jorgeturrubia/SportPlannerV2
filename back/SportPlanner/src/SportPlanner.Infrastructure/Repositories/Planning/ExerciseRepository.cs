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
            .Include(e => e.Category)
            .Include(e => e.Type)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<Exercise>> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Include(e => e.Category)
            .Include(e => e.Type)
            .Where(e => e.SubscriptionId == subscriptionId && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ExerciseDto>> GetExercisesDtoBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Where(e => e.SubscriptionId == subscriptionId && e.IsActive)
            .Include(e => e.Category)
            .Include(e => e.Type)
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseDto
            {
                Id = e.Id,
                SubscriptionId = e.SubscriptionId,
                Ownership = e.Ownership,
                Name = e.Name,
                Description = e.Description,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                TypeId = e.TypeId,
                TypeName = e.Type.Name,
                VideoUrl = e.VideoUrl,
                ImageUrl = e.ImageUrl,
                Instructions = e.Instructions,
                DefaultSets = e.DefaultSets,
                DefaultReps = e.DefaultReps,
                DefaultDurationSeconds = e.DefaultDurationSeconds,
                DefaultIntensity = e.DefaultIntensity,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Exercise>> GetSystemExercisesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Include(e => e.Category)
            .Include(e => e.Type)
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
}