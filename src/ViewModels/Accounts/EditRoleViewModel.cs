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

  public required List<SelectListItem> Roles { get; set; }
}
