using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class Image
{
  [Key]
  public required Guid Id { get; set; }

  public required string FileName { get; init; }

  public required int Width { get; init; }

  public required int Height { get; init; }

  public ApplicationUser? UploadedBy { get; set; }
}
