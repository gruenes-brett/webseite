using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Account;

public class ResetPasswordViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Email", ResourceType = typeof(SystemTexts))]
  public required string Email { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Password)]
  [StringLength(Constants.Account.MaximumPasswordLength, MinimumLength = Constants.Account.MinimumPasswordLength,
    ErrorMessageResourceName = "PasswordTooShortOrTooLong", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Password", ResourceType = typeof(SystemTexts))]
  public required string Password { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Password)]
  [Compare("Password", ErrorMessageResourceName = "PasswordRepeatMismatch", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "PasswordRepeat", ResourceType = typeof(SystemTexts))]
  public required string ConfirmPassword { get; set; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Token")]
  public required string Token { get; set; }
}
