namespace SportPlanner.Application.Common;

public static class DateTimeExtensions
{
    /// <summary>
    /// Ensures a DateTime is in UTC Kind for PostgreSQL timestamptz compatibility.
    /// PostgreSQL requires UTC DateTime objects for timestamp with time zone columns.
    /// </summary>
    public static DateTime ToUtcKind(this DateTime dt)
    {
        if (dt == default) return dt;

        // If it's already UTC, return as is
        if (dt.Kind == DateTimeKind.Utc) return dt;

        // If it's local time, convert to UTC
        if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();

        // If it's unspecified, assume it's in UTC and specify it
        // This handles ASP.NET Core model binding from ISO strings
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
}