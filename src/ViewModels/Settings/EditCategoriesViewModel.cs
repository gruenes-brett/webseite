using GruenesBrett.Models;

namespace GruenesBrett.ViewModels.Settings;

public class EditCategoriesViewModel
{
  public required IEnumerable<Category> Categories { get; set; }
}
