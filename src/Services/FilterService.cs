using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.ViewModels.Shared;

namespace GruenesBrett.Services;

public class FilterService(ILocationService locationService, IPostCodeService postCodeService,
  ICategoryService categoryService) : IFilterService
{
  /// <inheritdoc />
  public PostCode? GetCurrentPostCode(HttpRequest request)
  {
    if (request?.Cookies is null)
      return null;

    if (!request.Cookies.TryGetValue(Constants.Cookie.PostCode, out var postCodeCookie))
      return null;

    return postCodeService.GetPostCode(postCodeCookie);
  }

  /// <inheritdoc />
  public int GetCurrentSearchDistance(HttpRequest request)
  {
    if (request?.Cookies is null)
      return Constants.Locations.DefaultSearchDistance;

    if (!request.Cookies.TryGetValue(Constants.Cookie.SearchDistance, out var distanceCookie))
      return Constants.Locations.DefaultSearchDistance;

    if (!int.TryParse(distanceCookie, out var distanceValue))
      return Constants.Locations.DefaultSearchDistance;

    return ValidateSearchDistance(distanceValue);
  }

  /// <inheritdoc />
  public HashSet<Category> GetCurrentSelectedCategories(HttpRequest request)
  {
    if (request?.Cookies is null)
      return categoryService.GetAllCategoriesAsSet();

    if (!request.Cookies.TryGetValue(Constants.Cookie.SelectedCategories, out var selectedCategoriesCookie))
      return categoryService.GetAllCategoriesAsSet();

    var guids = selectedCategoriesCookie.GetGuids().ToList();
    if (guids.Count == 0)
      return categoryService.GetAllCategoriesAsSet();

    var selectedCategories = categoryService.GetCategories(guids);
    if (selectedCategories.Count == 0)
      return categoryService.GetAllCategoriesAsSet();

    return selectedCategories;
  }

  /// <inheritdoc />
  public string GetCurrentEventStatus(HttpRequest request)
  {
    if (request?.Cookies is null)
      return Constants.Status.AllStatuses;

    if (!request.Cookies.TryGetValue(Constants.Cookie.Status, out var statusCookie))
      return Constants.Status.AllStatuses;

    if (Constants.Status.All.Contains(statusCookie))
      return statusCookie;

    return Constants.Status.AllStatuses;
  }

  /// <inheritdoc />
  public int ValidateSearchDistance(int? searchDistance)
  {
    if (!searchDistance.HasValue)
      return Constants.Locations.DefaultSearchDistance;

    const int min = Constants.Locations.MinSearchDistance;
    const int max = Constants.Locations.MaxSearchDistance;
    return int.Clamp(searchDistance.Value, min, max);
  }

  /// <inheritdoc />
  public GetFilterViewModel GetFilterViewModel(HttpRequest request, string redirect)
  {
    var postCode = GetCurrentPostCode(request);
    var searchDistance = GetCurrentSearchDistance(request);
    var searchDistanceInMeters = locationService.GetSearchDistanceInMeters(searchDistance);
    var allCategories = categoryService.GetAllCategoriesSorted();
    var selectedCategories = GetCurrentSelectedCategories(request);

    return new GetFilterViewModel
    {
      PostCode = postCode,
      SearchDistance = searchDistance,
      SearchDistanceInMeters = searchDistanceInMeters,
      Categories = allCategories ?? [],
      SelectedCategories = selectedCategories ?? [],
      Redirect = redirect
    };
  }
}
