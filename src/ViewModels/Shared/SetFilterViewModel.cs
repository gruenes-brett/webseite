namespace GruenesBrett.ViewModels.Shared;

public class SetFilterViewModel
{
  public string? PostCode { get; init; }

  public int? SearchDistance { get; init; }

  public IEnumerable<Guid>? SelectedCategories { get; init; }

  public string? EventStatus { get; init; }

  public string? Redirect { get; init; }
}
