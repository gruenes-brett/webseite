using GruenesBrett.Models;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles post codes
/// </summary>
public interface IPostCodeService
{
  /// <summary>
  /// Returns the post code for the given key
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  PostCode? GetPostCode(string key);

  /// <summary>
  /// Returns all post codes
  /// </summary>
  /// <returns></returns>
  IEnumerable<PostCode> GetAllPostCodes();

  /// <summary>
  /// Returns all post codes matching the given substring
  /// </summary>
  /// <param name="query"></param>
  /// <returns></returns>
  IEnumerable<string> GetMatchingPostCodeNames(string query);
}
