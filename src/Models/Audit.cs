using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class Audit
{
  [Key]
  public required Guid Id { get; set; }

  public required string Message { get; set; }

  public required DateTime Timestamp { get; set; }

  public required string Section { get; set; }
}
