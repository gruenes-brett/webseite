using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class Audit
{
  [Key]
  public required Guid Id { get; init; }

  public required string Message { get; init; }

  public required DateTime Timestamp { get; init; }

  public required string Section { get; init; }
}
