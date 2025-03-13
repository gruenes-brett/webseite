using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Settings;

public class EditSettingsViewModel
{
  [Display(Name = "GlobalOptOut", ResourceType = typeof(SystemTexts))]
  public string? GlobalOptOut { get; set; }

  [Display(Name = "SelfRegistration", ResourceType = typeof(SystemTexts))]
  public bool SelfRegistration { get; set; }

  [Display(Name = "MaximumEventLength", ResourceType = typeof(SystemTexts))]
  public int MaximumEventLength { get; set; }
}
