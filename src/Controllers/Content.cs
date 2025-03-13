using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles content pages
/// </summary>
[Route("inhalte")]
public class Content() : Controller
{
  /// <summary>
  /// Shows the about us page
  /// </summary>
  /// <returns></returns>
  [Route("wer-wir-sind")]
  public IActionResult AboutUs()
  {
    return View();
  }

  /// <summary>
  /// Shows the report issue page
  /// </summary>
  /// <returns></returns>
  [Route("fehler-melden")]
  public IActionResult ReportIssue()
  {
    return View();
  }

  /// <summary>
  /// Shows the imprint page
  /// </summary>
  /// <returns></returns>
  [Route("impressum")]
  public IActionResult Imprint()
  {
    return View();
  }

  /// <summary>
  /// Shows the data privacy policy page
  /// </summary>
  /// <returns></returns>
  [Route("datenschutz")]
  public IActionResult DataPrivacyPolicy()
  {
    return View();
  }
}
