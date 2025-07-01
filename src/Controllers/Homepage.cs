using GruenesBrett.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles the homepage
/// </summary>
/// <param name="filterService"></param>
[Route("")]
public class Homepage(IFilterService filterService) : Controller
{
  /// <summary>
  /// Shows the homepage
  /// </summary>
  /// <param name="noRedirect"></param>
  /// <returns></returns>
  [Route("")]
  public IActionResult Index(bool noRedirect = false)
  {
    var postCode = filterService.GetCurrentPostCode(HttpContext.Request);
    if (postCode is null || noRedirect)
      return View();

    return RedirectToAction(nameof(Events.Explore), nameof(Events));
  }
}
