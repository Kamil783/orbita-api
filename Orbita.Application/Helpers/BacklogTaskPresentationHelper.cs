using System.Globalization;

namespace Orbita.Application.Helpers;

public static class BacklogTaskPresentationHelper
{
    public static string? GetDueDisplayText(DateTime? dueDate, DateTime now)
    {
        if (!dueDate.HasValue)
            return null;

        var date = dueDate.Value.Date;
        var today = now.Date;

        if (date == today)
            return "Сегодня";

        if (date == today.AddDays(1))
            return "Завтра";

        return date.ToString("dd MMMM", new CultureInfo("ru-RU"));
    }

    public static string? GetEstimateDisplayText(int? estimateMinutes)
    {
        if (!estimateMinutes.HasValue)
            return null;

        var totalMinutes = estimateMinutes.Value;
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        if (hours > 0 && minutes > 0)
            return $"{hours}ч {minutes}м";

        if (hours > 0)
            return $"{hours}ч";

        return $"{minutes}м";
    }
}
