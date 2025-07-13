using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Accounts;

public class EditResponsibilityViewModel
{
  public required string Name { get; init; }

  public required string Email { get; init; }

  public required string Role { get; init; }

  [Display(Name = "Radius", ResourceType = typeof(SystemTexts))]
  public int? Radius { get; init; }

  [Display(Name = "Longitude", ResourceType = typeof(SystemTexts))]
  public string? Longitude { get; init; }

  [Display(Name = "Latitude", ResourceType = typeof(SystemTexts))]
  public string? Latitude { get; init; }
}
