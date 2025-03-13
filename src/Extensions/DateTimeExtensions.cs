namespace GruenesBrett.Extensions;

/// <summary>
/// Extensions for date and time related classes
/// </summary>
public static class DateTimeExtensions
{
  /// <summary>
  /// Returns whether the given date should be considered a weekend or not
  /// </summary>
  /// <param name="dateOnly"></param>
  /// <returns></returns>
  internal static bool IsWeekend(this DateOnly dateOnly)
  {
    return dateOnly.DayOfWeek == DayOfWeek.Saturday || dateOnly.DayOfWeek == DayOfWeek.Sunday;
  }

  /// <summary>
  /// Returns whether the given date is today or not
  /// </summary>
  /// <param name="dateOnly"></param>
  /// <returns></returns>
  internal static bool IsToday(this DateOnly dateOnly)
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    return dateOnly == today;
  }

  /// <summary>
  /// Returns whether the given date is in the past or not
  /// </summary>
  /// <param name="dateOnly"></param>
  /// <returns></returns>
  internal static bool IsInThePast(this DateOnly dateOnly)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);
    return dateOnly < today;
  }
}
