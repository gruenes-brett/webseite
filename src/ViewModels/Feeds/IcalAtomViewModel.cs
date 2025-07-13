namespace GruenesBrett.ViewModels.Feeds;

public class IcalAtomViewModel
{
  public required string PostCode { get; init; }

  public required int SearchDistance { get; init; }

  public required IEnumerable<Guid> SelectedCategories { get; init; }
}
