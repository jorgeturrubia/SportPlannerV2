using SportPlanner.Domain.Entities.Planning;

namespace SportPlanner.Application.Interfaces;

public interface ICalendarEventRepository
{
    Task<List<CalendarEvent>> GetBySubscriptionIdAsync(
        Guid subscriptionId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? teamId = null,
        bool? isCompleted = null);

    Task<CalendarEvent?> GetByIdAsync(Guid id);

    Task<List<CalendarEvent>> GetByTeamAndDateRangeAsync(
        Guid teamId,
        DateTime startDate,
        DateTime endDate);

    Task<List<CalendarEvent>> GetByTrainingPlanIdAsync(Guid trainingPlanId);

    Task<bool> HasConflictAsync(
        Guid teamId,
        DateTime scheduledDate,
        int durationMinutes,
        Guid? excludeEventId = null);

    Task AddAsync(CalendarEvent calendarEvent);
    void Update(CalendarEvent calendarEvent);
    void Delete(CalendarEvent calendarEvent);
    Task SaveChangesAsync();
}
