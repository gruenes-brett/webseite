using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Account;

public class LoginViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Email", ResourceType = typeof(SystemTexts))]
  public required string Email { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Password)]
  [Display(Name = "Password", ResourceType = typeof(SystemTexts))]
  public required string Password { get; init; }

  [Display(Name = "RememberMe", ResourceType = typeof(SystemTexts))]
  public bool RememberMe { get; init; }

  public string? ReturnUrl { get; init; }

  public bool ConfirmEmailTrigger { get; init; }
}
