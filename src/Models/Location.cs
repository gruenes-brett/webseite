using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using GruenesBrett.Extensions;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Models;

public class Location
{
  [Key]
  public required Guid Id { get; init; }

  public required string Name { get; init; }

  public string? Address { get; init; }

  [JsonConverter(typeof(PointJsonConverter))]
  [Column(TypeName = "geography")]
  public required Point Coordinates { get; init; }
}
