using Microsoft.EntityFrameworkCore;
using SportPlanner.Application.Interfaces;
using SportPlanner.Domain.Entities.Planning;
using SportPlanner.Infrastructure.Data;

namespace SportPlanner.Infrastructure.Repositories.Planning;

public class CalendarEventRepository : ICalendarEventRepository
{
    private readonly SportPlannerDbContext _context;

    public CalendarEventRepository(SportPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<List<CalendarEvent>> GetBySubscriptionIdAsync(
        Guid subscriptionId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? teamId = null,
        bool? isCompleted = null)
    {
        var query = _context.CalendarEvents
            .Include(ce => ce.Workout)
            .Include(ce => ce.TrainingPlan)
            .Where(ce => ce.SubscriptionId == subscriptionId);

        if (startDate.HasValue)
            query = query.Where(ce => ce.ScheduledDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ce => ce.ScheduledDate <= endDate.Value);

        if (teamId.HasValue)
            query = query.Where(ce => ce.TeamId == teamId.Value);

        if (isCompleted.HasValue)
            query = query.Where(ce => ce.IsCompleted == isCompleted.Value);

        return await query
            .OrderBy(ce => ce.ScheduledDate)
            .ToListAsync();
    }

    public async Task<CalendarEvent?> GetByIdAsync(Guid id)
    {
        return await _context.CalendarEvents
            .Include(ce => ce.Workout)
            .Include(ce => ce.TrainingPlan)
            .FirstOrDefaultAsync(ce => ce.Id == id);
    }

    public async Task<List<CalendarEvent>> GetByTeamAndDateRangeAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate)
    {
        return await _context.CalendarEvents
            .Include(ce => ce.Workout)
            .Where(ce => ce.TeamId == teamId &&
                         ce.ScheduledDate >= startDate &&
                         ce.ScheduledDate <= endDate)
            .OrderBy(ce => ce.ScheduledDate)
            .ToListAsync();
    }

    public async Task<List<CalendarEvent>> GetByTrainingPlanIdAsync(Guid trainingPlanId)
    {
        return await _context.CalendarEvents
            .Include(ce => ce.Workout)
            .Where(ce => ce.TrainingPlanId == trainingPlanId)
            .OrderBy(ce => ce.ScheduledDate)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(
        Guid teamId,
        DateTime scheduledDate,
        int durationMinutes,
        Guid? excludeEventId = null)
    {
        var eventEnd = scheduledDate.AddMinutes(durationMinutes);

        var query = _context.CalendarEvents
            .Where(ce => ce.TeamId == teamId &&
                         ce.ScheduledDate < eventEnd &&
                         ce.ScheduledDate.AddMinutes(ce.DurationMinutes) > scheduledDate);

        if (excludeEventId.HasValue)
            query = query.Where(ce => ce.Id != excludeEventId.Value);

        return await query.AnyAsync();
    }

    public async Task AddAsync(CalendarEvent calendarEvent)
    {
        await _context.CalendarEvents.AddAsync(calendarEvent);
    }

    public void Update(CalendarEvent calendarEvent)
    {
        _context.CalendarEvents.Update(calendarEvent);
    }

    public void Delete(CalendarEvent calendarEvent)
    {
        _context.CalendarEvents.Remove(calendarEvent);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
