using System.ComponentModel.DataAnnotations;
using GruenesBrett.Models;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Events;

public class EditViewModel
{
  public required List<SingleEvent> Events { get; init; }

  public required IEnumerable<Category> Categories { get; init; }

  public required HashSet<Category> SelectedCategories { get; init; }

  [Display(Name = "EventStatus", ResourceType = typeof(SystemTexts))]
  public required string EventStatus { get; init; }

  public List<SelectListItem> Statuses { get; } =
    [.. Constants.Status.All.Select(s => new SelectListItem(Constants.Status.GetDisplayName(s), s))];
}
