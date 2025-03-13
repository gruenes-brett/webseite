using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Models;

public class PostCode
{
  [Key]
  public required string Id { get; set; }

  public required string FullName { get; set; }

  public required string CityName { get; set; }

  [Column(TypeName = "geography")]
  public required Point Coordinates { get; set; }
}
