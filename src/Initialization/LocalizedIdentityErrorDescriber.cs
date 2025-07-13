using GruenesBrett.Properties;
using Microsoft.AspNetCore.Identity;

namespace GruenesBrett.Initialization;

/// <summary>
/// Handles translation of identity related errors
/// </summary>
public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
  /// <inheritdoc />
  public override IdentityError DefaultError()
  {
    return new IdentityError
    {
      Code = nameof(DefaultError),
      Description = SystemTexts.DefaultError
    };
  }

  /// <inheritdoc />
  public override IdentityError ConcurrencyFailure()
  {
    return new IdentityError
    {
      Code = nameof(ConcurrencyFailure),
      Description = SystemTexts.ConcurrencyFailure
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordMismatch()
  {
    return new IdentityError
    {
      Code = nameof(PasswordMismatch),
      Description = SystemTexts.PasswordMismatch
    };
  }

  /// <inheritdoc />
  public override IdentityError InvalidToken()
  {
    return new IdentityError
    {
      Code = nameof(InvalidToken),
      Description = SystemTexts.InvalidToken
    };
  }

  /// <inheritdoc />
  public override IdentityError RecoveryCodeRedemptionFailed()
  {
    return new IdentityError
    {
      Code = nameof(RecoveryCodeRedemptionFailed),
      Description = SystemTexts.RecoveryCodeRedemptionFailed
    };
  }

  /// <inheritdoc />
  public override IdentityError LoginAlreadyAssociated()
  {
    return new IdentityError
    {
      Code = nameof(LoginAlreadyAssociated),
      Description = SystemTexts.LoginAlreadyAssociated
    };
  }

  /// <inheritdoc />
  public override IdentityError InvalidUserName(string? userName)
  {
    return new IdentityError
    {
      Code = nameof(InvalidUserName),
      Description = string.Format(SystemTexts.InvalidUserName, userName)
    };
  }

  /// <inheritdoc />
  public override IdentityError InvalidEmail(string? email)
  {
    return new IdentityError
    {
      Code = nameof(InvalidEmail),
      Description = string.Format(SystemTexts.InvalidEmail, email)
    };
  }

  /// <inheritdoc />
  public override IdentityError DuplicateUserName(string userName)
  {
    return new IdentityError
    {
      Code = nameof(DuplicateUserName),
      Description = string.Format(SystemTexts.DuplicateUserName, userName)
    };
  }

  /// <inheritdoc />
  public override IdentityError DuplicateEmail(string email)
  {
    return new IdentityError
    {
      Code = nameof(DuplicateEmail),
      Description = string.Format(SystemTexts.DuplicateEmail, email)
    };
  }

  /// <inheritdoc />
  public override IdentityError InvalidRoleName(string? role)
  {
    return new IdentityError
    {
      Code = nameof(InvalidRoleName),
      Description = string.Format(SystemTexts.InvalidRoleName, role)
    };
  }

  /// <inheritdoc />
  public override IdentityError DuplicateRoleName(string role)
  {
    return new IdentityError
    {
      Code = nameof(DuplicateRoleName),
      Description = string.Format(SystemTexts.DuplicateRoleName, role)
    };
  }

  /// <inheritdoc />
  public override IdentityError UserAlreadyHasPassword()
  {
    return new IdentityError
    {
      Code = nameof(UserAlreadyHasPassword),
      Description = SystemTexts.UserAlreadyHasPassword
    };
  }

  /// <inheritdoc />
  public override IdentityError UserLockoutNotEnabled()
  {
    return new IdentityError
    {
      Code = nameof(UserLockoutNotEnabled),
      Description = SystemTexts.UserLockoutNotEnabled
    };
  }

  /// <inheritdoc />
  public override IdentityError UserAlreadyInRole(string role)
  {
    return new IdentityError
    {
      Code = nameof(UserAlreadyInRole),
      Description = string.Format(SystemTexts.UserAlreadyInRole, role)
    };
  }

  /// <inheritdoc />
  public override IdentityError UserNotInRole(string role)
  {
    return new IdentityError
    {
      Code = nameof(UserNotInRole),
      Description = string.Format(SystemTexts.UserNotInRole, role)
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordTooShort(int length)
  {
    return new IdentityError
    {
      Code = nameof(PasswordTooShort),
      Description = string.Format(SystemTexts.PasswordTooShort, length)
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
  {
    return new IdentityError
    {
      Code = nameof(PasswordRequiresUniqueChars),
      Description = string.Format(SystemTexts.PasswordRequiresUniqueChars, uniqueChars)
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordRequiresNonAlphanumeric()
  {
    return new IdentityError
    {
      Code = nameof(PasswordRequiresNonAlphanumeric),
      Description = SystemTexts.PasswordRequiresNonAlphanumeric
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordRequiresDigit()
  {
    return new IdentityError
    {
      Code = nameof(PasswordRequiresDigit),
      Description = SystemTexts.PasswordRequiresDigit
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordRequiresLower()
  {
    return new IdentityError
    {
      Code = nameof(PasswordRequiresLower),
      Description = SystemTexts.PasswordRequiresLower
    };
  }

  /// <inheritdoc />
  public override IdentityError PasswordRequiresUpper()
  {
    return new IdentityError
    {
      Code = nameof(PasswordRequiresUpper),
      Description = SystemTexts.PasswordRequiresUpper
    };
  }
}
