namespace GruenesBrett.ViewModels.Feeds;

public class IcalAtomViewModel
{
  public required string PostCode { get; set; }

  public required int SearchDistance { get; set; }

  public required IEnumerable<Guid> SelectedCategories { get; set; }
}
