using GruenesBrett.Models;
using Microsoft.AspNetCore.Identity;

namespace GruenesBrett.Initialization;

/// <summary>
/// Handles logging out banned users
/// </summary>
/// <param name="next"></param>
public class LogOutBannedUsersMiddleware(RequestDelegate next)
{
  /// <summary>
  /// Checks the currently logged-in user and logs them out when they are banned
  /// </summary>
  /// <param name="context"></param>
  /// <param name="userManager"></param>
  /// <param name="signInManager"></param>
  /// <returns></returns>
  public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager)
  {
    var email = context.User?.Identity?.Name;
    if (email is not null)
    {
      var user = await userManager.FindByEmailAsync(email);
      if (user?.Banned != false)
        await signInManager.SignOutAsync();
    }

    await next(context);
  }
}

/// <summary>
/// Handles configuring the middleware
/// </summary>
public static class LogOutBannedUsersMiddlewareExtensions
{
  /// <summary>
  /// Configures the given builder to user the middleware
  /// </summary>
  /// <param name="builder"></param>
  /// <returns></returns>
  public static IApplicationBuilder UseLogOutBannedUsers(this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<LogOutBannedUsersMiddleware>();
  }
}
