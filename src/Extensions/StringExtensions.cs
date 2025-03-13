using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace GruenesBrett.Extensions;

/// <summary>
/// Extensions related to strings
/// </summary>
public static partial class StringExtensions
{
  /// <summary>
  /// Truncates the given string to a maximum length and adds
  /// the given truncation character if truncation was necessary
  /// </summary>
  /// <param name="s"></param>
  /// <param name="length"></param>
  /// <param name="character"></param>
  /// <returns></returns>
  internal static string? Truncate(this string s, int length, char character = '\u2026')
  {
    if (s is null || s.Length <= length)
      return s;

    var lastWhitespace = s.LastIndexOf(' ', length);
    if (lastWhitespace > 0)
      return $"{s[..lastWhitespace]}{character}";

    return $"{s[..length]}{character}";
  }

  /// <summary>
  /// Regex for finding all values in curly braces
  /// </summary>
  /// <returns></returns>
  [GeneratedRegex("{.*?}")]
  private static partial Regex ReplaceNamedParametersRegex();

  /// <summary>
  /// Replaces all values in curly braces with the given values
  /// </summary>
  /// <param name="s"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public static string FormatWith(this string s, params object?[] args)
  {
    if (s.IsNullOrEmpty())
      return s;

    var regex = ReplaceNamedParametersRegex();
    var index = 0;
    var replaced = regex.Replace(s, _ => $"{{{index++}}}");
    return string.Format(replaced, args);
  }

  /// <summary>
  /// Returns whether the given string is null or empty or not
  /// </summary>
  /// <param name="s"></param>
  /// <returns></returns>
  public static bool IsNullOrEmpty([NotNullWhen(false)] this string? s)
  {
    return string.IsNullOrEmpty(s);
  }

  /// <summary>
  /// Returns whether the given string is null or only consisting of whitespaces or not
  /// </summary>
  /// <param name="s"></param>
  /// <returns></returns>
  public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? s)
  {
    return string.IsNullOrWhiteSpace(s);
  }

  /// <summary>
  /// Returns whether the given string is not null or not empty or not
  /// </summary>
  /// <param name="s"></param>
  /// <returns></returns>
  public static bool HasValue([NotNullWhen(true)] this string? s)
  {
    return !string.IsNullOrEmpty(s);
  }

  /// <summary>
  /// Returns the GUIDs from the given comma-separated list
  /// </summary>
  /// <param name="s"></param>
  /// <returns></returns>
  public static IEnumerable<Guid> GetGuids(this string? s, char separator = ',')
  {
    if (s.IsNullOrEmpty())
      yield break;

    var splitValues = s.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    foreach (var splitValue in splitValues)
    {
      if (Guid.TryParse(splitValue, out Guid guid))
        yield return guid;
    }
  }
}
