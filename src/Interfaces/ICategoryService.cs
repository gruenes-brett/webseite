using GruenesBrett.Models;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles retrieving and storing categories
/// </summary>
public interface ICategoryService
{
  /// <summary>
  /// Returns all categories sorted alphabetically
  /// </summary>
  /// <returns></returns>
  List<Category> GetAllCategoriesSorted();

  /// <summary>
  /// Returns all categories as a set
  /// </summary>
  /// <returns></returns>
  HashSet<Category> GetAllCategoriesAsSet();

  /// <summary>
  /// Returns all categories for the given GUIDs as a set
  /// </summary>
  /// <param name="ids"></param>
  /// <returns></returns>
  HashSet<Category> GetCategories(List<Guid> ids);

  /// <summary>
  /// Reads all categories from the database into cache
  /// </summary>
  void RefreshAllCategories();
}
