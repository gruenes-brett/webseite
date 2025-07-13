using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Event;

public class ReportViewModel
{
  public required string EventId { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Reason", ResourceType = typeof(SystemTexts))]
  public required string Reason { get; init; }

  public required IEnumerable<SelectListItem> Reasons { get; init; }
}
