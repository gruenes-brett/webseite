using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Accounts;

public class EditResponsibilityViewModel
{
  public required string Name { get; set; }

  public required string Email { get; set; }

  public required string Role { get; set; }

  [Display(Name = "Radius", ResourceType = typeof(SystemTexts))]
  public int? Radius { get; set; }

  [Display(Name = "Longitude", ResourceType = typeof(SystemTexts))]
  public string? Longitude { get; set; }

  [Display(Name = "Latitude", ResourceType = typeof(SystemTexts))]
  public string? Latitude { get; set; }
}
