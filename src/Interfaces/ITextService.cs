using GruenesBrett.Models;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles editorial texts
/// </summary>
public interface ITextService
{
  /// <summary>
  /// Returns the text value for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  string GetText(string key);

  /// <summary>
  /// Returns the text information for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  Task<Text?> GetTextAsync(string key);

  /// <summary>
  /// Sets the given value for the text for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  Task SetTextAsync(string key, string value);

  /// <summary>
  /// Returns all texts and their keys
  /// </summary>
  /// <returns></returns>
  IEnumerable<KeyValuePair<string, string>> GetAllTexts();

  /// <summary>
  /// Returns the date and time for the given event in human-readable form
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  string GetFormattedDateTimeSpan(SingleEvent singleEvent);

  /// <summary>
  /// Returns the time for the given event in human-readable form
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  string GetFormattedTimeSpan(SingleEvent singleEvent);

  /// <summary>
  /// Returns the start date and time in machine-readable form
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  string GetFormattedStartDateAndTime(SingleEvent singleEvent);

  /// <summary>
  /// Returns the end date and time in machine-readable form
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  string GetFormattedEndDateAndTime(SingleEvent singleEvent);
}
