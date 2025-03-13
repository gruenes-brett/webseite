using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;

namespace GruenesBrett.Services;

public class SettingsService(ApplicationDbContext context) : ISettingsService
{
  /// <inheritdoc />
  public async Task<string> GetStringSettingAsync(string key)
  {
    var setting = await context.Settings.FindAsync(key);
    if (setting is null)
      return string.Empty;

    return setting.Value;
  }

  /// <inheritdoc />
  public async Task<HashSet<string>> GetSetSettingAsync(string key)
  {
    var value = await GetStringSettingAsync(key);
    if (value.IsNullOrEmpty())
      return [];

    var entries = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    return [.. entries];
  }

  /// <inheritdoc />
  public async Task<bool> GetBoolSettingAsync(string key)
  {
    var value = await GetStringSettingAsync(key);
    return value == "true";
  }

  /// <inheritdoc />
  public async Task<int> GetIntSettingAsync(string key)
  {
    var value = await GetStringSettingAsync(key);
    return int.TryParse(value, out var result) ? result : 0;
  }

  /// <inheritdoc />
  public async Task SetStringSettingAsync(string key, string value)
  {
    var setting = await context.Settings.FindAsync(key);
    if (setting is null)
    {
      var newSetting = new Setting { Key = key, Value = value };
      context.Add(newSetting);
      await context.SaveChangesAsync();
    }
    else
    {
      setting.Value = value;
      await context.SaveChangesAsync();
    }
  }

  /// <inheritdoc />
  public async Task SetBoolSettingAsync(string key, bool value)
  {
    await SetStringSettingAsync(key, value ? "true" : "false");
  }

  /// <inheritdoc />
  public async Task SetIntSettingAsync(string key, int value)
  {
    await SetStringSettingAsync(key, value.ToString());
  }
}
