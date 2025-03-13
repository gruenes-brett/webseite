using GruenesBrett.ViewModels.Shared;

namespace GruenesBrett.ViewModels.Feeds;

public class FeedsViewModel
{
  public required string ApiUrl { get; set; }

  public required string AtomUrl { get; set; }

  public required string IcalUrl { get; set; }

  public required GetFilterViewModel FilterViewModel { get; set; }
}
