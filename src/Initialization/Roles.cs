using Microsoft.AspNetCore.Identity;

namespace GruenesBrett.Initialization;

/// <summary>
/// Handles creating all roles during initialization
/// </summary>
public static class Roles
{
  /// <summary>
  /// Creates all roles when necessary
  /// </summary>
  /// <param name="roleManager"></param>
  /// <returns></returns>
  public static async Task EnsureCreatedAsync(RoleManager<IdentityRole> roleManager)
  {
    var roleNames = Constants.Roles.All;
    foreach (var roleName in roleNames)
    {
      if (await roleManager.FindByNameAsync(roleName) is null)
      {
        var role = new IdentityRole(roleName);
        await roleManager.CreateAsync(role);
      }
    }
  }
}
