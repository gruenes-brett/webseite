using GruenesBrett.Models;
using GruenesBrett.ViewModels.Shared;

namespace GruenesBrett.ViewModels.Events;

public class ExploreViewModel
{
  public required List<SingleEvent> Events { get; init; }

  public required GetFilterViewModel FilterViewModel { get; init; }
}
