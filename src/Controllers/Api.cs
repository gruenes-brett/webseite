using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles the public REST API endpoints
/// </summary>
/// <param name="eventService"></param>
/// <param name="locationService"></param>
/// <param name="postCodeService"></param>
[ApiController]
[Tags("API")]
[Route("api/[action]")]
public class Api(IEventService eventService, ILocationService locationService, IPostCodeService postCodeService) : Controller
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
}
