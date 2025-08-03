using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Nominatim.API.Interfaces;
using Nominatim.API.Models;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles the public REST API endpoints
/// </summary>
/// <param name="eventService"></param>
/// <param name="forwardGeocoder"></param>
/// <param name="locationService"></param>
/// <param name="postCodeService"></param>
[ApiController]
[Tags("API")]
[Route("api/[action]")]
public class Api(IEventService eventService, IForwardGeocoder forwardGeocoder , ILocationService locationService,
  IPostCodeService postCodeService) : Controller
{
  /// <summary>
  /// Returns the event for the given ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [EndpointSummary("Event")]
  [EndpointDescription("Returns the event for the given ID.")]
  [ProducesResponseType(typeof(ApiEvent), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [HttpGet]
  public async Task<IActionResult> Event(
    [Required]
    string eventId)
  {
    var approvedEvent = await eventService.GetApprovedEventAsync(eventId);
    if (approvedEvent is null)
      return NotFound();

    var mappedEvent = new ApiEvent(approvedEvent);
    return Json(mappedEvent);
  }

  /// <summary>
  /// Returns the events matching the given parameters
  /// </summary>
  /// <param name="postCodeName"></param>
  /// <param name="searchDistance"></param>
  /// <returns></returns>
  [EndpointSummary("Events")]
  [EndpointDescription("Returns the events matching the given parameters.")]
  [ProducesResponseType(typeof(IEnumerable<ApiEvent>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [HttpGet]
  public async Task<IActionResult> Events(
    [Required]
    [Description("Can be only the numerical part or the numerical part and the name of the city (for example \"01097\" or \"01097 Dresden\").")]
    string postCodeName,
    [Required]
    [Description("Must be a number between 1 and 5 (inclusive, 1 being a radius of 10km and 5 being a radius of 150km).")]
    byte searchDistance
    )
  {
    var postCode = postCodeService.GetPostCode(postCodeName);
    if (postCode == null)
    {
      var problemDetails = new ValidationProblemDetails();
      problemDetails.Errors.Add("postCodeName", ["The value for the parameter was invalid."]);
      return BadRequest(problemDetails);
    }

    var coordinates = postCode.Coordinates;
    var searchDistanceInMeters = locationService.GetSearchDistanceInMeters(searchDistance);
    if (searchDistanceInMeters == 0)
    {
      var problemDetails = new ValidationProblemDetails();
      problemDetails.Errors.Add("searchDistance", ["The value for the parameter was invalid."]);
      return BadRequest(problemDetails);
    }

    var approvedEvents = await eventService.GetApprovedEventsAsync([], coordinates, searchDistanceInMeters);
    var mappedEvents = approvedEvents.Select(approvedEvent => new ApiEvent(approvedEvent));
    return Json(mappedEvents);
  }

  /// <summary>
  /// Returns the list of post codes / ZIP codes matching the given query
  /// </summary>
  /// <param name="query"></param>
  /// <returns></returns>
  [EndpointSummary("Post Codes")]
  [EndpointDescription("Returns the list of post codes / ZIP codes matching the given query.")]
  [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [HttpGet]
  public IActionResult PostCodes(
    [Required]
    [Description("Can be part of a post code (for example \"010\") or a city name (for example \"Dres\").")]
    string query)
  {
    var matchingPostCodeNames = postCodeService.GetMatchingPostCodeNames(query);
    return Json(matchingPostCodeNames);
  }

  /// <summary>
  /// Returns the list of addresses matching the given query
  /// </summary>
  /// <param name="query"></param>
  /// <returns></returns>
  [EndpointSummary("Addresses")]
  [EndpointDescription("Returns the list of addresses matching the given query.")]
  [ProducesResponseType(typeof(IEnumerable<ApiAddress>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
  [HttpGet]
  [EnableRateLimiting(Constants.System.OneSecondRateLimit)]
  public async Task<IActionResult> Addresses(
    [Required]
    [Description("Can be part of an address (for example \"Alaunstr\") or a city name (for example \"Dresden\").")]
    string query)
  {
    if (query.IsNullOrEmpty())
    {
      var problemDetails = new ValidationProblemDetails();
      problemDetails.Errors.Add("query", ["The value for the parameter was invalid."]);
      return BadRequest(problemDetails);
    }
    
    var request = new ForwardGeocodeRequest
    {
      queryString = query,
      CountryCodeSearch = "de",
      BreakdownAddressElements = true
    };

    var response = await forwardGeocoder.Geocode(request);
    if (response is null)
      return Json(Enumerable.Empty<ApiAddress>());

    var results = response.Select(MapResponseToAddress);
    var uniqueResults = results.DistinctBy(a => a.Name);
    return Json(uniqueResults);
  }

  /// <summary>
  /// Returns the mapped address for a given geocode response
  /// </summary>
  /// <param name="response"></param>
  /// <returns></returns>
  private static ApiAddress MapResponseToAddress(GeocodeResponse response)
  {
    var latitude = response.Latitude;
    var longitude = response.Longitude;
    
    var name = new StringBuilder();
    if (response.Address.Road.HasValue())
      name.Append(response.Address.Road);
    if (response.Address.Road.HasValue() && response.Address.HouseNumber.HasValue())
      name.Append(' ');
    if (response.Address.HouseNumber.HasValue())
      name.Append(response.Address.HouseNumber);
    if (response.Address.Road.HasValue())
      name.Append(", ");
    if (response.Address.PostCode.HasValue())
      name.Append(response.Address.PostCode);
    if (response.Address.PostCode.HasValue() &&
        (response.Address.City.HasValue() || response.Address.Town.HasValue() || response.Address.Village.HasValue()))
      name.Append(' ');
    if (response.Address.City.HasValue())
      name.Append(response.Address.City);
    else if (response.Address.Town.HasValue())
      name.Append(response.Address.Town);
    else if (response.Address.Village.HasValue())
      name.Append(response.Address.Village);

    return new ApiAddress(name.ToString(), latitude, longitude);
  }
}
