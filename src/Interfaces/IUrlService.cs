using GruenesBrett.Models;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles URLs
/// </summary>
public interface IUrlService
{
  /// <summary>
  /// Returns the permalink for the given event
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  string? GetPermalink(SingleEvent singleEvent);

  /// <summary>
  /// Returns the URL encoded permalink for the given event
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  string? GetEncodedPermalink(SingleEvent singleEvent);

  /// <summary>
  /// Returns the absolute URL to the given controller and action
  /// </summary>
  /// <param name="controller"></param>
  /// <param name="action"></param>
  /// <param name="values"></param>
  /// <returns></returns>
  string GetAbsoluteUrl(string controller, string action, object? values = null);

  /// <summary>
  /// Returns the absolute URL for the given relative URL
  /// </summary>
  /// <param name="relativeUrl"></param>
  /// <returns></returns>
  string GetAbsoluteUrl(string relativeUrl);
}
