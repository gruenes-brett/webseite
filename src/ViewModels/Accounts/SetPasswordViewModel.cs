using System.ComponentModel.DataAnnotations;
using GruenesBrett.Properties;

namespace GruenesBrett.ViewModels.Accounts;

public class SetPasswordViewModel
{
  public required string UserId { get; set; }

  public required string Code { get; set; }

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
}
