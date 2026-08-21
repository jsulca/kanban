using System.Globalization;

namespace Kanban.SharedKernel;

public static class DateTimeExtensions
{
    private static readonly GregorianCalendar _gc = new();

    public static int GetWeekOfMonth(this DateTime time)
    {
        var first = new DateTime(time.Year, time.Month, 1);
        return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
    }

    private static int GetWeekOfYear(this DateTime time)
    {
        return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }
}