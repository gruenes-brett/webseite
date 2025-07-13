using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Account;

public class AccountSettingsViewModel
{
  [Display(Name = "ReceiveOptionalEmails", ResourceType = typeof(SystemTexts))]
  public bool ReceiveOptionalEmails { get; init; }

  [Display(Name = "ReceiveReportingEmails", ResourceType = typeof(SystemTexts))]
  public bool ReceiveReportingEmails { get; init; }

  [Display(Name = "DefaultPage", ResourceType = typeof(SystemTexts))]
  public string? DefaultPage { get; init; }

  public List<SelectListItem> DefaultPages { get; } =
    [.. Constants.Pages.All.Select(p => new SelectListItem(Constants.Pages.GetDisplayName(p), p))];
}
