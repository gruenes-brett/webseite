using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Accounts;

public class EditRoleViewModel
{
  public required string Name { get; set; }

  public required string Email { get; set; }

  public required DateOnly Created { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Role", ResourceType = typeof(SystemTexts))]
  public required string Role { get; set; }

  public List<SelectListItem> Roles { get; } =
    [.. Constants.Roles.All.Select(r => new SelectListItem(Constants.Roles.GetDisplayName(r), r))];
}
