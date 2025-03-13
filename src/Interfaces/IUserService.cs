using System.Security.Claims;
using GruenesBrett.Models;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles users
/// </summary>
public interface IUserService
{
  /// <summary>
  /// Returns the user for the given principal
  /// </summary>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal);

  /// <summary>
  /// Returns the internal name of the role of the given user
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  Task<string?> GetRoleAsync(ApplicationUser user);

  /// <summary>
  /// Returns the external name of the role of the given user
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  Task<string?> GetRoleDisplayNameAsync(ApplicationUser user);

  /// <summary>
  /// Returns all administrators and editors that are responsible for the given event
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  Task<List<ApplicationUser>> GetRelevantUsersAsync(SingleEvent singleEvent);

  /// <summary>
  /// Returns whether there is currently only one user registered or not
  /// </summary>
  /// <returns></returns>
  Task<bool> IsFirstUser();
}
