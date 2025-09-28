using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories;

public class GenderRepository : IGenderRepository
{
    private readonly SportPlannerDbContext _context;

    public GenderRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<Gender?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Genders
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Gender?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Genders
            .FirstOrDefaultAsync(g => g.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<List<Gender>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Genders
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Genders
            .AnyAsync(g => g.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task AddAsync(Gender gender, CancellationToken cancellationToken = default)
    {
        await _context.Genders.AddAsync(gender, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Gender gender, CancellationToken cancellationToken = default)
    {
        _context.Genders.Update(gender);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Gender gender, CancellationToken cancellationToken = default)
    {
        _context.Genders.Remove(gender);
        await _context.SaveChangesAsync(cancellationToken);
    }
}