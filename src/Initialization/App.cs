using GruenesBrett.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace GruenesBrett.Initialization;

/// <summary>
/// Handles configuration of the app
/// </summary>
internal static class App
{
  /// <summary>
  /// Configures all functionality for the app
  /// </summary>
  /// <param name="app"></param>
  /// <returns></returns>
  internal static async Task ConfigureAsync(WebApplication app)
  {
    // automatically apply all available database migrations and create roles if necessary
    await using (var scope = app.Services.CreateAsyncScope())
    await using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
    using (var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>())
    {
      Console.WriteLine("Starting database migration");
      await context.Database.MigrateAsync();
      Console.WriteLine("Finished database migration");
      Console.WriteLine("Starting role seeding");
      await Roles.EnsureCreatedAsync(roleManager);
      Console.WriteLine("Finished role seeding");
    }

    // configure reverse proxy handling
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
      ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    // configure the exception pages
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseMigrationsEndPoint();
      app.UseStaticFiles();
    }
    else
    {
      app.UseExceptionHandler("/fehler");
    }

    app.UseStatusCodePagesWithReExecute("/fehler/{0}");

    app.UseAuthorization();

    //app.UseSerilogRequestLogging();

    app.MapControllers();

    app.MapOpenApi();
    app.MapScalarApiReference(Constants.System.ApiReference, options =>
    {
      options.CustomCss = "body { --scalar-color-accent: #5ea318; }";
      options.DarkMode = false;
      options.DefaultFonts = false;
      options.Favicon = "/favicon.png";
      options.Title = "API Reference - Grünes Brett";
    });

    app.UseLogOutBannedUsers();
  }
}
