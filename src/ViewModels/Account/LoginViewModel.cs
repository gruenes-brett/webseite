using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Account;

public class LoginViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Email", ResourceType = typeof(SystemTexts))]
  public required string Email { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Password)]
  [Display(Name = "Password", ResourceType = typeof(SystemTexts))]
  public required string Password { get; set; }

  [Display(Name = "RememberMe", ResourceType = typeof(SystemTexts))]
  public bool RememberMe { get; set; }

  public string? ReturnUrl { get; set; }

  public bool ConfirmEmailTrigger { get; set; }
}
