using GruenesBrett.Models;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles writing audit logs
/// </summary>
public interface IAuditService
{
  /// <summary>
  /// Logs account related activity
  /// </summary>
  /// <param name="message"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  Task LogAccountActivityAsync(string? message, params object?[] args);

  /// <summary>
  /// Logs event related activity
  /// </summary>
  /// <param name="message"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  Task LogEventActivityAsync(string? message, params object?[] args);

  /// <summary>
  /// Returns the newest audit entries for the given section
  /// </summary>
  /// <param name="section"></param>
  /// <returns></returns>
  Task<List<Audit>> GetNewestActivities(string section);
}
