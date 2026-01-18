using System.Security.Claims;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GruenesBrett.Services;

public class UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : IUserService
{
  /// <inheritdoc />
  public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal principal)
  {
    if (principal is null)
      return null;

    return await userManager.GetUserAsync(principal);
  }

  /// <inheritdoc />
  public async Task<string?> GetRoleAsync(ApplicationUser user)
  {
    var roles = await userManager.GetRolesAsync(user);
    return roles.FirstOrDefault();
  }

  /// <inheritdoc />
  public async Task<string?> GetRoleDisplayNameAsync(ApplicationUser user)
  {
    var role = await GetRoleAsync(user);
    if (role is null)
      return null;

    return Constants.Roles.GetDisplayName(role);
  }

  /// <inheritdoc />
  public async Task<List<ApplicationUser>> GetRelevantUsersAsync(SingleEvent singleEvent)
  {
    var users = await userManager.Users
      .Where(u => singleEvent.EventLocation.Coordinates.IsWithinDistance(u.Coordinates, u.Radius * 1000))
      .ToListAsync();

    var result = new List<ApplicationUser>();

    foreach (var user in users)
    {
      var role = await GetRoleAsync(user);
      if (role.HasValue() && role != Constants.Roles.Normal)
        result.Add(user);
    }

    return result;
  }

  /// <inheritdoc />
  public async Task<bool> IsFirstUser()
  {
    return await context.Users.CountAsync() == 1;
  }

  /// <inheritdoc />
  public async Task<bool> HasNoUsers()
  {
    return await context.Users.CountAsync() == 0;
  }
}
