using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Extensions;

/// <summary>
/// An attribute to indicate that the checkbox must be checked
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MustBeCheckedAttribute : ValidationAttribute
{
  /// <summary>
  /// Returns whether the value of the given checkbox indicates that it is checked
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public override bool IsValid(object? value) => value is true;
}
