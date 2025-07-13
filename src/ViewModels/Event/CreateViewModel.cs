using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Event;

public class CreateViewModel : CreateOrEditViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "CreatedByName", ResourceType = typeof(SystemTexts))]
  public required string CreatedByName { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "CreatedByEmail", ResourceType = typeof(SystemTexts))]
  public required string CreatedByEmail { get; init; }
}
