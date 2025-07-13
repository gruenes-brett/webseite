using GruenesBrett.ViewModels.Shared;

namespace GruenesBrett.ViewModels.Feeds;

public class FeedsViewModel
{
  public required string ApiUrl { get; init; }

  public required string AtomUrl { get; init; }

  public required string IcalUrl { get; init; }

  public required GetFilterViewModel FilterViewModel { get; init; }
}
