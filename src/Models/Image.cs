using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class Image
{
  [Key]
  public required Guid Id { get; set; }

  public required string FileName { get; set; }

  public required int Width { get; set; }

  public required int Height { get; set; }

  public ApplicationUser? UploadedBy { get; set; }
}
