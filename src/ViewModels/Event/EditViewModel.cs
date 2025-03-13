using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Event;

public class EditViewModel : CreateOrEditViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  public required Guid EventId { get; set; }

  public string? PreviousImage { get; set; }
}
