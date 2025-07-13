using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class Text
{
  [Key]
  public required string Key { get; init; }

  public required string Value { get; set; }

  public required bool IsRichText { get; set; }
}
