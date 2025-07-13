using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GruenesBrett.Models;

public class Category
{
  [Key]
  public required Guid Id { get; init; }

  public required string Name { get; set; }

  [JsonIgnore]
  public string? ForegroundColor { get; set; }

  [JsonIgnore]
  public string? BackgroundColor { get; set; }
}
