using System.Security.Claims;
using GruenesBrett.Models;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles retrieving events
/// </summary>
public interface IEventService
{
  /// <summary>
  /// Returns the approved event for the given external ID
  /// </summary>
  /// <param name="externalId"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetApprovedEventAsync(string externalId, ClaimsPrincipal? principal = null);

  /// <summary>
  /// Returns the approved event for the given internal ID
  /// </summary>
  /// <param name="internalId"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetApprovedEventAsync(Guid internalId, ClaimsPrincipal? principal = null);

  /// <summary>
  /// Returns the draft event for the given external ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="externalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetDraftEventAsync(string externalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the draft event for the given internal ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="internalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetDraftEventAsync(Guid internalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the rejected event for the given external ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="externalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetRejectedEventAsync(string externalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the rejected event for the given internal ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="internalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetRejectedEventAsync(Guid internalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the approved or draft event for the given internal ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="internalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetApprovedOrDraftEventAsync(Guid internalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the draft or rejected event for the given external ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="externalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetDraftOrRejectedEventAsync(string externalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the draft or rejected event for the given internal ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="internalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetDraftOrRejectedEventAsync(Guid internalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the approved, draft or rejected event for the given internal ID
  /// (using the access rights of the given principal)
  /// </summary>
  /// <param name="internalId"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<SingleEvent?> GetApprovedOrDraftOrRejectedEventAsync(Guid internalId, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the approved events
  /// (matching the given categories, coordinates and search distance)
  /// </summary>
  /// <param name="categories"></param>
  /// <returns></returns>
  Task<List<SingleEvent>> GetApprovedEventsAsync(HashSet<Category> categories, Point? coordinates = null, double? searchDistanceInMeters = null, ClaimsPrincipal? principal = null);

  /// <summary>
  /// Returns the draft events
  /// (matching the given categories and using the access rights of the given principal)
  /// </summary>
  /// <param name="categories"></param>
  /// <returns></returns>
  Task<List<SingleEvent>> GetDraftEventsAsync(HashSet<Category> categories, ClaimsPrincipal principal);

  /// <summary>
  /// Returns the rejected events
  /// (matching the given categories and using the access rights of the given principal)
  /// </summary>
  /// <param name="categories"></param>
  /// <returns></returns>
  Task<List<SingleEvent>> GetRejectedEventsAsync(HashSet<Category> categories, ClaimsPrincipal principal);

  /// <summary>
  /// Returns whether the given user can edit the given event
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  Task<bool> CanEditEventAsync(SingleEvent singleEvent, ClaimsPrincipal principal);

  /// <summary>
  /// Returns whether an event with the given external ID exists
  /// </summary>
  /// <param name="externalId"></param>
  /// <returns></returns>
  Task<bool> EventExists(string externalId);
}
