using GruenesBrett.Models;
using GruenesBrett.ViewModels.Shared;

namespace GruenesBrett.ViewModels.Events;

public class CalendarViewModel
{
  public required OrderedDictionary<DateOnly, List<SingleEvent>> DateEvents { get; set; }

  public required GetFilterViewModel FilterViewModel { get; set; }
}
