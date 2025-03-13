using GruenesBrett.Models;

namespace GruenesBrett.ViewModels.Shared;

public class GetFilterViewModel
{
  public PostCode? PostCode { get; set; }

  public int SearchDistance { get; set; }

  public double SearchDistanceInMeters { get; set; }

  public required IEnumerable<Category> Categories { get; set; }

  public required HashSet<Category> SelectedCategories { get; set; }

  public string? Redirect { get; set; }
}
