namespace GruenesBrett.Models;

/// <summary>
/// An address in the REST API
/// </summary>
public class ApiAddress(string name, double latitude, double longitude)
{
  public string Name { get; init; } = name;

  public double Latitude { get; init; } = latitude;

  public double Longitude { get; init; } = longitude;
}
