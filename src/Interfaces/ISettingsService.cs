namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles site-wide settings
/// </summary>
public interface ISettingsService
{
  /// <summary>
  /// Returns the value of the setting for the given key as a string
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  Task<string> GetStringSettingAsync(string key);

  /// <summary>
  /// Returns the value of the setting for the given key as a set of strings
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  Task<HashSet<string>> GetSetSettingAsync(string key);

  /// <summary>
  /// Returns the value of the setting for the given key as a bool
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  Task<bool> GetBoolSettingAsync(string key);

  /// <summary>
  /// Returns the value of the setting for the given key as an int
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  Task<int> GetIntSettingAsync(string key);

  /// <summary>
  /// Sets the given value for the setting for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  Task SetStringSettingAsync(string key, string value);

  /// <summary>
  /// Sets the given value for the setting for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  Task SetBoolSettingAsync(string key, bool value);

  /// <summary>
  /// Sets the given value for the setting for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  Task SetIntSettingAsync(string key, int value);
}
