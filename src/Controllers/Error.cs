using GruenesBrett.ViewModels.Error;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles error pages
/// </summary>
[Route("fehler")]
public class Error : Controller
{
  /// <summary>
  /// Shows the page for the error with the given status code
  /// </summary>
  /// <param name="statusCode"></param>
  /// <returns></returns>
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  [Route("{statusCode}")]
  public IActionResult Index(int statusCode)
  {
    var viewModel = new ErrorViewModel
    {
      StatusCode = statusCode
    };
    return View(viewModel);
  }
}
