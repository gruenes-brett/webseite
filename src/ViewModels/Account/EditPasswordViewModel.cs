using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Account;

public class EditPasswordViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Password)]
  [StringLength(Constants.Account.MaximumPasswordLength, MinimumLength = Constants.Account.MinimumPasswordLength,
    ErrorMessageResourceName = "PasswordTooShortOrTooLong", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Password", ResourceType = typeof(SystemTexts))]
  public required string Password { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Password)]
  [Compare("Password", ErrorMessageResourceName = "PasswordRepeatMismatch", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "PasswordRepeat", ResourceType = typeof(SystemTexts))]
  public required string ConfirmPassword { get; init; }
}
