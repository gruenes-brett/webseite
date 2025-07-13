using GruenesBrett.Models;

namespace GruenesBrett.ViewModels.Shared;

public class GetFilterViewModel
{
  public PostCode? PostCode { get; init; }

  public int SearchDistance { get; init; }

  public double SearchDistanceInMeters { get; init; }

  public required IEnumerable<Category> Categories { get; init; }

  public required HashSet<Category> SelectedCategories { get; init; }

  public string? Redirect { get; init; }
}
