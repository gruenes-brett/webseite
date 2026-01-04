using System.Collections.Concurrent;
using System.Text;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.Properties;

namespace GruenesBrett.Services;

public class TextService : ITextService
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILogger<TextService> _logger;
  private readonly ConcurrentDictionary<string, string> _cache = new();

  /// <summary>
  /// Constructor for initializing the cache
  /// </summary>
  /// <param name="scopeFactory"></param>
  /// <param name="logger"></param>
  public TextService(IServiceScopeFactory scopeFactory, ILogger<TextService> logger)
  {
    _scopeFactory = scopeFactory;
    _logger = logger;
    RefreshAllTexts();
  }

  /// <inheritdoc />
  public string GetText(string key)
  {
    _cache.TryGetValue(key, out var text);

    if (text.IsNullOrEmpty())
      _logger.LogWarning("Could not find text for key {key}", key);

    return text ?? string.Empty;
  }

  /// <inheritdoc />
  public async Task<Text?> GetTextAsync(string key)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Texts.FindAsync(key);
  }

  /// <inheritdoc />
  public async Task SetTextAsync(string key, string value)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var text = await context.Texts.FindAsync(key);
    if (text is not null)
    {
      text.Value = value;
      _cache.AddOrUpdate(key, value, (_, _) => value);
      await context.SaveChangesAsync();
    }
  }

  /// <inheritdoc />
  public IEnumerable<KeyValuePair<string, string>> GetAllTexts()
  {
    var enumerable = _cache.AsEnumerable();
    return enumerable.OrderBy(kv => kv.Key);
  }

  /// <inheritdoc />
  public string GetFormattedDateTimeSpan(SingleEvent singleEvent)
  {
    var result = new StringBuilder();
    var startDate = $"{singleEvent.StartDate:ddd} {singleEvent.StartDate:dd.MM.yyyy}";
    result.Append(startDate);

    if (singleEvent.EndDate is not null && singleEvent.StartDate != singleEvent.EndDate)
    {
      var endDate = $"{singleEvent.EndDate:ddd} {singleEvent.EndDate:dd.MM.yyyy}";
      result.Append($" {SystemTexts.Until} {endDate}");
    }

    var formattedTimeSpan = GetFormattedTimeSpan(singleEvent);
    if (formattedTimeSpan.HasValue())
    {
      result.Append(", ");
      result.Append("<time>");
      result.Append(formattedTimeSpan);
      result.Append("</time>");
    }

    return result.ToString();
  }

  /// <inheritdoc />
  public string GetFormattedTimeSpan(SingleEvent singleEvent)
  {
    var result = new StringBuilder();

    if (singleEvent.StartTime is not null)
    {
      var startTime = $"{singleEvent.StartTime:HH:mm}";
      result.Append(startTime);
    }

    if (singleEvent.EndTime is not null && singleEvent.StartTime != singleEvent.EndTime)
    {
      var endTime = $"{singleEvent.EndTime:HH:mm}";
      result.Append($" \u2013 {endTime}");
    }

    if (singleEvent.StartTime is not null)
      result.Append($" {SystemTexts.Clock}");

    return result.ToString();
  }

  /// <inheritdoc />
  public string GetFormattedStartDateAndTime(SingleEvent singleEvent)
  {
    var start = singleEvent.StartTime is not null
      ? new DateTime(singleEvent.StartDate, singleEvent.StartTime.Value)
      : new DateTime(singleEvent.StartDate, TimeOnly.MinValue);

    return start.ToString("yyyyMMddTHHmmss");
  }

  /// <inheritdoc />
  public string GetFormattedEndDateAndTime(SingleEvent singleEvent)
  {
    var end = singleEvent.EndDate is not null
      ? singleEvent.EndTime is not null
        ? new DateTime(singleEvent.EndDate.Value, singleEvent.EndTime.Value)
        : new DateTime(singleEvent.EndDate.Value, TimeOnly.MinValue)
      : singleEvent.EndTime is not null
        ? new DateTime(singleEvent.StartDate, singleEvent.EndTime.Value)
        : DateTime.MinValue;

    var formattedEndDateAndTime = end == DateTime.MinValue ? string.Empty : end.ToString("yyyyMMddTHHmmss");
    return formattedEndDateAndTime;
  }

  /// <summary>
  /// Reads all texts from the database into the cache
  /// </summary>
  private void RefreshAllTexts()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    foreach (var text in context.Texts)
      _cache.AddOrUpdate(text.Key, text.Value, (_, _) => text.Value);
  }
}
