using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Models;

public class PostCode
{
  [Key]
  public required string Id { get; init; }

  public required string FullName { get; init; }

  public required string CityName { get; init; }

  [Column(TypeName = "geography")]
  public required Point Coordinates { get; init; }
}
