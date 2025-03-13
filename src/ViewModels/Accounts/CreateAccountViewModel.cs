using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Accounts;

public class CreateAccountViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Name", ResourceType = typeof(SystemTexts))]
  public required string Name { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Email", ResourceType = typeof(SystemTexts))]
  public required string Email { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Role", ResourceType = typeof(SystemTexts))]
  public required string Role { get; set; }

  public List<SelectListItem> Roles { get; } =
    [.. Constants.Roles.All.Select(r => new SelectListItem(Constants.Roles.GetDisplayName(r), r))];
}
