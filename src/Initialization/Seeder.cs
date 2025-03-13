using System.Collections;
using System.Globalization;
using GruenesBrett.Extensions;
using GruenesBrett.Models;
using GruenesBrett.Properties;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Initialization;

/// <summary>
/// Handles seeding values in the database during initialization
/// </summary>
public static class Seeder
{
  /// <summary>
  /// Creates default categories in the database if none exist already
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  public static async Task SeedCategoriesAsync(DbContext context)
  {
    var categoryCount = await context.Set<Category>().CountAsync();
    var categoriesAlreadyExist = categoryCount > 0;
    if (categoriesAlreadyExist)
      return;

    var resourceSet = Categories.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
    if (resourceSet is null)
      return;

    var categories = resourceSet.Cast<DictionaryEntry>().Select(x =>
    {
      var name = x.Key?.ToString() ?? string.Empty;
      var colors = x.Value?.ToString() ?? string.Empty;
      var splitColors = colors.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

      var foregroundColor = splitColors.Length > 0 ? splitColors[0] : "#000000";
      var backgroundColor = splitColors.Length > 1 ? splitColors[1] : "#ffffff";

      return new Category()
      {
        Id = Guid.CreateVersion7(),
        Name = name,
        ForegroundColor = foregroundColor,
        BackgroundColor = backgroundColor
      };
    });

    await context.AddRangeAsync(categories);
    await context.SaveChangesAsync();
  }

  /// <summary>
  /// Creates and updates all post codes in the database
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  public static async Task SeedPostCodesAsync(DbContext context)
  {
    var postCodeCount = await context.Set<PostCode>().CountAsync();
    var postCodesAlreadyExist = postCodeCount > 0;
    if (postCodesAlreadyExist)
      return;

    var resourceSet = PostCodes.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
    if (resourceSet is null)
      return;

    var postCodes = resourceSet.Cast<DictionaryEntry>().Select(x =>
    {
      var fullName = x.Key?.ToString() ?? string.Empty;
      var coordinateValue = x.Value?.ToString() ?? string.Empty;

      var id = fullName[..5];
      var cityName = fullName[6..];

      var splitCoordinates = coordinateValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

      var hasLatitude = double.TryParse(splitCoordinates[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var latitude);
      var hasLongitude = double.TryParse(splitCoordinates[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var longitude);
      var coordinates = new Point(longitude, latitude) { SRID = Constants.Locations.Srid };

      return new PostCode()
      {
        Id = id,
        CityName = cityName,
        FullName = fullName,
        Coordinates = coordinates
      };
    });

    await context.AddRangeAsync(postCodes);
    await context.SaveChangesAsync();
  }

  /// <summary>
  /// Creates all editorial texts in the database if they do not exist already
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  public static async Task SeedTextsAsync(DbContext context)
  {
    var resourceSet = Texts.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
    if (resourceSet is null)
      return;

    var texts = resourceSet.Cast<DictionaryEntry>().Select(x =>
    {
      var key = x.Key?.ToString();
      var value = x.Value?.ToString();

      return new Text()
      {
        Key = key ?? string.Empty,
        Value = value ?? string.Empty,
        IsRichText = key is not null && Constants.Text.IsRichText(key)
      };
    });

    if (texts is null)
      return;

    foreach (var text in texts)
    {
      if (text.Key.IsNullOrEmpty())
        continue;

      var existingText = await context.Set<Text>().FindAsync(text.Key);
      if (existingText is null)
      {
        context.Set<Text>().Add(text);
      }
      else
      {
        existingText.IsRichText = text.IsRichText;
      }
    }

    await context.SaveChangesAsync();
  }
}
