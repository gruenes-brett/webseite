using System.ComponentModel.DataAnnotations;
using GruenesBrett.Models;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Events;

public class EditViewModel
{
  public required List<SingleEvent> Events { get; set; }

  public required IEnumerable<Category> Categories { get; set; }

  public required HashSet<Category> SelectedCategories { get; set; }

  [Display(Name = "EventStatus", ResourceType = typeof(SystemTexts))]
  public required string EventStatus { get; set; }

  public List<SelectListItem> Statuses { get; } =
    [.. Constants.Status.All.Select(s => new SelectListItem(Constants.Status.GetDisplayName(s), s))];
}
