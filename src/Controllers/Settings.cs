using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles managing site-wide settings
/// </summary>
/// <param name="categoryService"></param>
/// <param name="context"></param>
/// <param name="settingsService"></param>
/// <param name="textService"></param>
[Authorize(Roles = Constants.Roles.Administrator)]
[Route("einstellungen")]
public class Settings(ICategoryService categoryService, ApplicationDbContext context, ISettingsService settingsService,
  ITextService textService) : Controller
{
  /// <summary>
  /// Shows the form for site-wide settings
  /// </summary>
  /// <returns></returns>
  [Route("")]
  public async Task<IActionResult> EditSettings()
  {
    var globalOptOut = await settingsService.GetStringSettingAsync(Constants.Settings.GlobalOptOut);
    var maximumEventLength = await settingsService.GetIntSettingAsync(Constants.Settings.MaximumEventLength);
    var selfRegistration = await settingsService.GetBoolSettingAsync(Constants.Settings.SelfRegistration);
    var viewModel = new EditSettingsViewModel
    {
      GlobalOptOut = globalOptOut,
      MaximumEventLength = maximumEventLength,
      SelfRegistration = selfRegistration
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for site-wide settings
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("")]
  public async Task<IActionResult> EditSettings(EditSettingsViewModel viewModel)
  {
    await settingsService.SetStringSettingAsync(Constants.Settings.GlobalOptOut, viewModel.GlobalOptOut ?? string.Empty);
    await settingsService.SetIntSettingAsync(Constants.Settings.MaximumEventLength, viewModel.MaximumEventLength);
    await settingsService.SetBoolSettingAsync(Constants.Settings.SelfRegistration, viewModel.SelfRegistration);
    return View();
  }

  /// <summary>
  /// Shows the list of all categories
  /// </summary>
  /// <returns></returns>
  [Route("kategorien-bearbeiten")]
  public IActionResult EditCategories()
  {
    var categories = categoryService.GetAllCategoriesSorted();
    var viewModel = new EditCategoriesViewModel
    {
      Categories = categories
    };
    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for editing a category
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [Route("kategorie-bearbeiten")]
  public async Task<IActionResult> EditCategory(Guid id)
  {
    var category = await context.Categories.FindAsync(id);
    if (category is null)
      return RedirectToAction(nameof(EditCategories));

    var viewModel = new EditCategoryViewModel
    {
      Id = category.Id,
      Name = category.Name,
      ForegroundColor = category.ForegroundColor ?? string.Empty,
      BackgroundColor = category.BackgroundColor ?? string.Empty
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles inputs for the form for editing a category
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("kategorie-bearbeiten")]
  public async Task<IActionResult> EditCategory(EditCategoryViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View(viewModel);

    var category = await context.Categories.FindAsync(viewModel.Id);
    if (category is null)
      return RedirectToAction(nameof(EditCategories));

    category.Name = viewModel.Name;
    category.ForegroundColor = viewModel.ForegroundColor;
    category.BackgroundColor = viewModel.BackgroundColor;

    await context.SaveChangesAsync();
    categoryService.RefreshAllCategories();
    return RedirectToAction(nameof(EditCategories));
  }

  /// <summary>
  /// Shows the form for creating a new category
  /// </summary>
  /// <returns></returns>
  [Route("kategorie-anlegen")]
  public IActionResult AddCategory()
  {
    var viewModel = new AddCategoryViewModel
    {
      Name = string.Empty,
      ForegroundColor = "#ffffff",
      BackgroundColor = "#000000"
    };

    return View(viewModel);
  }

  /// <summary>
  /// Handles inputs for the form for creating a new category
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("kategorie-anlegen")]
  public async Task<IActionResult> AddCategory(AddCategoryViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View(viewModel);

    var category = new Category
    {
      Id = Guid.CreateVersion7(),
      Name = viewModel.Name,
      ForegroundColor = viewModel.ForegroundColor,
      BackgroundColor = viewModel.BackgroundColor
    };

    context.Categories.Add(category);
    await context.SaveChangesAsync();
    categoryService.RefreshAllCategories();
    return RedirectToAction(nameof(EditCategories));
  }

  /// <summary>
  /// Shows the form for deleting a category
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [Route("kategorie-loeschen")]
  public async Task<IActionResult> DeleteCategory(Guid id)
  {
    var category = await context.Categories.FindAsync(id);
    if (category is null)
      return RedirectToAction(nameof(EditCategories));

    var viewModel = new DeleteCategoryViewModel
    {
      Id = category.Id,
      Name = category.Name
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for deleting a category
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("kategorie-loeschen")]
  public async Task<IActionResult> DeleteCategory(DeleteCategoryViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View(viewModel);

    var category = await context.Categories.FindAsync(viewModel.Id);
    if (category is null)
      return RedirectToAction(nameof(EditCategories));

    context.Categories.Remove(category);
    await context.SaveChangesAsync();
    categoryService.RefreshAllCategories();
    return RedirectToAction(nameof(EditCategories));
  }

  /// <summary>
  /// Shows a list of all editorial texts
  /// </summary>
  /// <returns></returns>
  [Route("texte-bearbeiten")]
  public IActionResult EditTexts()
  {
    var texts = textService.GetAllTexts();
    var viewModel = new EditTextsViewModel
    {
      Texts = texts
    };
    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for editing an editorial text
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  [Route("text-bearbeiten")]
  public async Task<IActionResult> EditText(string key)
  {
    var text = await textService.GetTextAsync(key);
    if (text is null)
      return RedirectToAction(nameof(EditTexts));

    var viewModel = new EditTextViewModel
    {
      Key = text.Key,
      Value = text.Value,
      IsRichText = text.IsRichText
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for editing an editorial text
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("text-bearbeiten")]
  public async Task<IActionResult> EditText(EditTextViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View(viewModel);

    await textService.SetTextAsync(viewModel.Key, viewModel.Value);
    return RedirectToAction(nameof(EditTexts));
  }
}
