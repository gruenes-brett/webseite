using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Settings;

public class AddCategoryViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Name", ResourceType = typeof(SystemTexts))]
  public required string Name { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "ForegroundColor", ResourceType = typeof(SystemTexts))]
  public required string ForegroundColor { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "BackgroundColor", ResourceType = typeof(SystemTexts))]
  public required string BackgroundColor { get; init; }
}
