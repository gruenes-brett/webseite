using GruenesBrett.Interfaces;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Services;

public class LocationService : ILocationService
{
  /// <inheritdoc />
  public bool IsValidCoordinates(Point coordinates)
  {
    if (coordinates.Y is < Constants.Locations.MinLatitude or > Constants.Locations.MaxLatitude)
      return false;

    if (coordinates.X is < Constants.Locations.MinLongitude or > Constants.Locations.MaxLongitude)
      return false;

    return true;
  }

  /// <inheritdoc />
  public double GetSearchDistanceInMeters(int searchDistance) => searchDistance switch
  {
    1 => 10_000,  //  10 km
    2 => 15_000,  //  15 km
    3 => 20_000,  //  20 km
    4 => 75_000,  //  75 km
    5 => 150_000, // 150 km
    _ => 0,
  };
}
