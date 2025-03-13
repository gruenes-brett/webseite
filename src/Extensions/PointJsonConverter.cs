using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Extensions;

/// <summary>
/// A converter to write coordinates to JSON
/// </summary>
public class PointJsonConverter : JsonConverter<Point>
{
  /// <summary>
  /// Reads a given JSON part and returns it as a coordinate
  /// </summary>
  /// <param name="reader"></param>
  /// <param name="typeToConvert"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// Writes the given coordinate to the given JSON writer
  /// </summary>
  /// <param name="writer"></param>
  /// <param name="point"></param>
  /// <param name="options"></param>
  public override void Write(Utf8JsonWriter writer, Point point, JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    writer.WriteNumber("longitude", point.X);
    writer.WriteNumber("latitude", point.Y);
    writer.WriteEndObject();
  }
}
