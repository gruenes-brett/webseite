using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class Setting
{
  [Key]
  public required string Key { get; init; }

  public required string Value { get; set; }
}
