namespace GruenesBrett.ViewModels.Shared;

public class SetFilterViewModel
{
  public string? PostCode { get; set; }

  public int? SearchDistance { get; set; }

  public IEnumerable<Guid>? SelectedCategories { get; set; }

  public string? EventStatus { get; set; }

  public string? Redirect { get; set; }
}
