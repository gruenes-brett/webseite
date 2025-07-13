using GruenesBrett.Models;
using GruenesBrett.ViewModels.Shared;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles reading and validating the used search filters
/// </summary>
public interface IFilterService
{
  /// <summary>
  /// Returns the currently selected post code
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  PostCode? GetCurrentPostCode(HttpRequest request);

  /// <summary>
  /// Returns the currently selected search distance
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  int GetCurrentSearchDistance(HttpRequest request);

  /// <summary>
  /// Returns the currently selected categories
  /// (or all categories if none were selected)
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  HashSet<Category> GetCurrentSelectedCategories(HttpRequest request);

  /// <summary>
  /// Returns the currently selected event status
  /// </summary>
  /// <param name="request"></param>
  /// <returns></returns>
  string GetCurrentEventStatus(HttpRequest request);

  /// <summary>
  /// Clamps the given search distance to a valid range and returns it
  /// </summary>
  /// <param name="searchDistance"></param>
  /// <returns></returns>
  int ValidateSearchDistance(int? searchDistance);

  /// <summary>
  /// Reads the currently selected filters from the cookies and returns them
  /// </summary>
  /// <param name="request"></param>
  /// <param name="redirect"></param>
  /// <returns></returns>
  GetFilterViewModel GetFilterViewModel(HttpRequest request, string redirect);
}
