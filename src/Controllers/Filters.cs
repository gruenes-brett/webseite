using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers
{
  /// <summary>
  /// Handles filters
  /// </summary>
  /// <param name="filterService"></param>
  /// <param name="postCodeService"></param>
  [Route("filters")]
  public class Filters(IFilterService filterService, IPostCodeService postCodeService) : Controller
  {
    /// <summary>
    /// Saves the given filters to cookies and redirects to the given page
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("filter-setzen")]
    public IActionResult SetFilter(SetFilterViewModel viewModel)
    {
      if (viewModel.PostCode.HasValue())
      {
        var foundPostCode = postCodeService.GetPostCode(viewModel.PostCode);
        if (foundPostCode is not null)
          HttpContext.Response.Cookies.Append(Constants.Cookie.PostCode, foundPostCode.Id, Constants.Cookie.Options);
      }

      if (viewModel.SearchDistance is not null)
      {
        var distance = filterService.ValidateSearchDistance(viewModel.SearchDistance);
        HttpContext.Response.Cookies.Append(Constants.Cookie.SearchDistance, distance.ToString(), Constants.Cookie.Options);
      }

      if (viewModel.SelectedCategories is not null)
      {
        var selectedCategories = string.Join(",", viewModel.SelectedCategories);
        HttpContext.Response.Cookies.Append(Constants.Cookie.SelectedCategories, selectedCategories, Constants.Cookie.Options);
      }

      if (viewModel.EventStatus is not null)
      {
        var status = Constants.Status.All.Contains(viewModel.EventStatus) ? viewModel.EventStatus : Constants.Status.AllStatuses;
        HttpContext.Response.Cookies.Append(Constants.Cookie.Status, status, Constants.Cookie.Options);
      }

      var allowedRoutes = new List<string>
      {
        $"{nameof(Events)}/{nameof(Events.Calendar)}",
        $"{nameof(Events)}/{nameof(Events.Explore)}",
        $"{nameof(Events)}/{nameof(Events.Index)}",
        $"{nameof(Feeds)}/{nameof(Feeds.Index)}"
      };

      if (viewModel.Redirect.HasValue() && allowedRoutes.Contains(viewModel.Redirect))
      {
        var redirect = viewModel.Redirect.Split('/');
        return RedirectToAction(redirect[1], redirect[0]);
      }

      return Redirect("/");
    }
  }
}
