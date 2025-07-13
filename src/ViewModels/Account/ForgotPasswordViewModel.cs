using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Account;

public class ForgotPasswordViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Email", ResourceType = typeof(SystemTexts))]
  public required string Email { get; init; }
}
