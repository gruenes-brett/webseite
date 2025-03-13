using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles the homepage
/// </summary>
[Route("")]
public class Homepage() : Controller
{
  /// <summary>
  /// Shows the homepage
  /// </summary>
  /// <returns></returns>
  [Route("")]
  public IActionResult Index()
  {
    return View();
  }
}
