using NetTopologySuite.Geometries;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles validating locations
/// </summary>
public interface ILocationService
{
  /// <summary>
  /// Returns whether the given coordinates are within the bounds
  /// of the area that is permitted on the site
  /// </summary>
  /// <param name="coordinates"></param>
  /// <returns></returns>
  bool IsValidCoordinates(Point coordinates);

  /// <summary>
  /// Returns the search distance in meters from the given search distance level
  /// </summary>
  /// <param name="searchDistance"></param>
  /// <returns></returns>
  double GetSearchDistanceInMeters(int searchDistance);
}
