using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Accounts;

public class EditRoleViewModel
{
  public required string Name { get; init; }

  public required string Email { get; init; }

  public required DateOnly Created { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Role", ResourceType = typeof(SystemTexts))]
  public required string Role { get; init; }

  public List<SelectListItem> Roles { get; } =
    [.. Constants.Roles.All.Select(r => new SelectListItem(Constants.Roles.GetDisplayName(r), r))];
}
