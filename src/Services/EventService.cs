using System.Security.Claims;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Services;

public class EventService(ApplicationDbContext context, IUserService userService) : IEventService
{
  /// <inheritdoc />
  public async Task<SingleEvent?> GetApprovedEventAsync(string externalId, ClaimsPrincipal? principal = null)
  {
    var approvedEvents = await GetApprovedEventsAsync(principal);
    if (approvedEvents is null)
      return null;

    return await approvedEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetApprovedEventAsync(Guid internalId, ClaimsPrincipal? principal = null)
  {
    var approvedEvents = await GetApprovedEventsAsync(principal);
    if (approvedEvents is null)
      return null;

    return await approvedEvents.FirstOrDefaultAsync(e => e.InternalId == internalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetPastApprovedEventAsync(string externalId, ClaimsPrincipal? principal = null)
  {
    var pastApprovedEvents = await GetPastApprovedEventsAsync(principal);
    if (pastApprovedEvents is null)
      return null;

    return await pastApprovedEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetDraftEventAsync(string externalId, ClaimsPrincipal principal)
  {
    var draftEvents = await GetDraftEventsAsync(principal);
    if (draftEvents is null)
      return null;

    return await draftEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetDraftEventAsync(Guid internalId, ClaimsPrincipal principal)
  {
    var draftEvents = await GetDraftEventsAsync(principal);
    if (draftEvents is null)
      return null;

    return await draftEvents.FirstOrDefaultAsync(e => e.InternalId == internalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetRejectedEventAsync(string externalId, ClaimsPrincipal principal)
  {
    var rejectedEvents = await GetRejectedEventsAsync(principal);
    if (rejectedEvents is null)
      return null;

    return await rejectedEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetRejectedEventAsync(Guid internalId, ClaimsPrincipal principal)
  {
    var rejectedEvents = await GetRejectedEventsAsync(principal);
    if (rejectedEvents is null)
      return null;

    return await rejectedEvents.FirstOrDefaultAsync(e => e.InternalId == internalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetApprovedOrDraftEventAsync(Guid internalId, ClaimsPrincipal principal)
  {
    var approvedEvent = await GetApprovedEventAsync(internalId, principal);
    return approvedEvent ?? await GetDraftEventAsync(internalId, principal);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetDraftOrRejectedEventAsync(string externalId, ClaimsPrincipal principal)
  {
    var draftEvent = await GetDraftEventAsync(externalId, principal);
    return draftEvent ?? await GetRejectedEventAsync(externalId, principal);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetDraftOrRejectedEventAsync(Guid internalId, ClaimsPrincipal principal)
  {
    var draftEvent = await GetDraftEventAsync(internalId, principal);
    return draftEvent ?? await GetRejectedEventAsync(internalId, principal);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetEventAsync(string externalId, ClaimsPrincipal principal)
  {
    var allEvents = await GetAllEventsAsync(principal);
    if (allEvents is null)
      return null;

    return await allEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetEventAsync(Guid internalId, ClaimsPrincipal principal)
  {
    var allEvents = await GetAllEventsAsync(principal);
    if (allEvents is null)
      return null;

    return await allEvents.FirstOrDefaultAsync(e => e.InternalId == internalId);
  }

  /// <inheritdoc />
  public async Task<SingleEvent?> GetPastEventAsync(string externalId, ClaimsPrincipal principal)
  {
    var pastEvents = await GetPastEventsAsync(principal);
    if (pastEvents is null)
      return null;

    return await pastEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
  }

  /// <inheritdoc />
  public async Task<List<SingleEvent>> GetApprovedEventsAsync(HashSet<Category> categories, Point? coordinates = null,
    double? searchDistanceInMeters = null, ClaimsPrincipal? principal = null)
  {
    var approvedEvents = await GetApprovedEventsAsync(principal);
    if (approvedEvents is null)
      return [];

    approvedEvents = ApplyCategoriesFilter(approvedEvents, categories);
    approvedEvents = ApplyLocationFilter(approvedEvents, coordinates, searchDistanceInMeters);
    approvedEvents = ApplyOrdering(approvedEvents);
    return await approvedEvents.ToListAsync();
  }

  /// <inheritdoc />
  public async Task<List<SingleEvent>> GetDraftEventsAsync(HashSet<Category> categories, ClaimsPrincipal principal)
  {
    var draftEvents = await GetDraftEventsAsync(principal);
    if (draftEvents is null)
      return [];

    draftEvents = ApplyCategoriesFilter(draftEvents, categories);
    draftEvents = ApplyOrdering(draftEvents);
    return await draftEvents.ToListAsync();
  }

  /// <inheritdoc />
  public async Task<List<SingleEvent>> GetRejectedEventsAsync(HashSet<Category> categories, ClaimsPrincipal principal)
  {
    var rejectedEvents = await GetRejectedEventsAsync(principal);
    if (rejectedEvents is null)
      return [];

    rejectedEvents = ApplyCategoriesFilter(rejectedEvents, categories);
    return await rejectedEvents.ToListAsync();
  }

  /// <inheritdoc />
  public async Task<List<SingleEvent>> GetPastEventsAsync(HashSet<Category> categories, ClaimsPrincipal principal)
  {
    var pastEvents = await GetPastEventsAsync(principal);
    if (pastEvents is null)
      return [];

    pastEvents = ApplyCategoriesFilter(pastEvents, categories);
    return await pastEvents.ToListAsync();
  }

  /// <inheritdoc />
  public async Task<bool> CanEditEventAsync(SingleEvent singleEvent, ClaimsPrincipal principal)
  {
    if (principal is null)
      return false;

    var user = await userService.GetUserAsync(principal);
    if (user is null)
      return false;

    var isAdministrator = principal.IsInRole(Constants.Roles.Administrator);
    if (isAdministrator)
      return true;

    var isEditor = principal.IsInRole(Constants.Roles.Editor);
    var isChiefEditor = principal.IsInRole(Constants.Roles.ChiefEditor);
    if (isEditor || isChiefEditor)
    {
      var radius = user.Radius * 1000;
      if (singleEvent.EventLocation.Coordinates.IsWithinDistance(user.Coordinates, radius))
        return true;

      return singleEvent.CreatedBy?.Equals(user) == true;
    }

    var isNormal = principal.IsInRole(Constants.Roles.Normal);
    if (isNormal)
      return singleEvent.CreatedBy?.Equals(user) == true;

    return false;
  }

  /// <inheritdoc />
  public async Task<bool> EventExists(string externalId)
  {
    var singleEvent = await context.SingleEvents.FirstOrDefaultAsync(e => e.ExternalId == externalId);
    return singleEvent is not null;
  }

  /// <inheritdoc />
  public bool EventWasInThePast(SingleEvent singleEvent)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);
    return (singleEvent.EndDate is not null && singleEvent.EndDate < today) || (singleEvent.EndDate is null && singleEvent.StartDate < today);
  }

  /// <summary>
  /// Returns a queryable for events in the approved workflow status
  /// (with all the necessary includes and filters already applied)
  /// </summary>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>?> GetApprovedEventsAsync(ClaimsPrincipal? principal)
  {
    var approvedEvents = context.SingleEvents.AsQueryable();
    if (principal is not null)
      approvedEvents = await ApplyUserFilter(approvedEvents, principal);
    approvedEvents = ApplyWorkflowFilter(approvedEvents, WorkflowStatus.Approved);
    approvedEvents = ApplyDateFilter(approvedEvents);
    approvedEvents = ApplyIncludes(approvedEvents);
    return approvedEvents;
  }

  /// <summary>
  /// Returns a queryable for events in the past and in the approved workflow status
  /// (with all the necessary includes and filters already applied)
  /// </summary>
  /// <param name="principal"></param>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>?> GetPastApprovedEventsAsync(ClaimsPrincipal? principal)
  {
    var pastApprovedEvents = context.SingleEvents.AsQueryable();
    if (principal is not null)
      pastApprovedEvents = await ApplyUserFilter(pastApprovedEvents, principal);
    pastApprovedEvents = ApplyWorkflowFilter(pastApprovedEvents, WorkflowStatus.Approved);
    pastApprovedEvents = ApplyPastDateFilter(pastApprovedEvents);
    pastApprovedEvents = ApplyIncludes(pastApprovedEvents);
    return pastApprovedEvents;
  }

  /// <summary>
  /// Returns a queryable for events in the draft workflow status
  /// (with all the necessary includes and filters already applied)
  /// </summary>
  /// <param name="principal"></param>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>?> GetDraftEventsAsync(ClaimsPrincipal principal)
  {
    var draftEvents = context.SingleEvents.AsQueryable();
    draftEvents = await ApplyUserFilter(draftEvents, principal);
    draftEvents = ApplyWorkflowFilter(draftEvents, WorkflowStatus.Draft);
    draftEvents = ApplyDateFilter(draftEvents);
    draftEvents = ApplyIncludes(draftEvents);
    return draftEvents;
  }

  /// <summary>
  /// Returns a queryable for events in the rejected workflow status
  /// (with all the necessary includes and filters already applied)
  /// </summary>
  /// <param name="principal"></param>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>?> GetRejectedEventsAsync(ClaimsPrincipal principal)
  {
    var rejectedEvents = context.SingleEvents.AsQueryable();
    rejectedEvents = await ApplyUserFilter(rejectedEvents, principal);
    rejectedEvents = ApplyWorkflowFilter(rejectedEvents, WorkflowStatus.Rejected);
    rejectedEvents = ApplyDateFilter(rejectedEvents);
    rejectedEvents = ApplyIncludes(rejectedEvents);
    return rejectedEvents;
  }

  /// <summary>
  /// Returns a queryable for events in the past
  /// (with all the necessary includes and filters already applied)
  /// </summary>
  /// <param name="principal"></param>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>?> GetPastEventsAsync(ClaimsPrincipal principal)
  {
    var pastEvents = context.SingleEvents.AsQueryable();
    pastEvents = await ApplyUserFilter(pastEvents, principal);
    pastEvents = ApplyPastDateFilter(pastEvents);
    pastEvents = ApplyIncludes(pastEvents);
    return pastEvents;
  }

  /// <summary>
  /// Returns a queryable for all events
  /// (with all the necessary includes and filters already applied)
  /// </summary>
  /// <param name="principal"></param>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>?> GetAllEventsAsync(ClaimsPrincipal principal)
  {
    var allEvents = context.SingleEvents.AsQueryable();
    allEvents = await ApplyUserFilter(allEvents, principal);
    allEvents = ApplyIncludes(allEvents);
    return allEvents;
  }

  /// <summary>
  /// Applies the filter for the given principal to the given source
  /// </summary>
  /// <param name="source"></param>
  /// <param name="principal"></param>
  /// <returns></returns>
  private async Task<IQueryable<SingleEvent>> ApplyUserFilter(IQueryable<SingleEvent> source, ClaimsPrincipal? principal)
  {
    if (principal?.Identity is null)
      return source.Where(_ => false);

    if (!principal.Identity.IsAuthenticated)
      return source.Where(_ => false);

    var user = await userService.GetUserAsync(principal);
    if (user is null)
      return source.Where(_ => false);

    var isAdministrator = principal.IsInRole(Constants.Roles.Administrator);
    if (isAdministrator)
      return source;

    var isEditor = principal.IsInRole(Constants.Roles.Editor);
    var isChiefEditor = principal.IsInRole(Constants.Roles.ChiefEditor);
    if (isEditor || isChiefEditor)
    {
      var radius = user.Radius * 1000;
      return source.Where(e => e.EventLocation.Coordinates.IsWithinDistance(user.Coordinates, radius) || (e.CreatedBy != null && e.CreatedBy.Equals(user)));
    }

    var isNormal = principal.IsInRole(Constants.Roles.Normal);
    if (isNormal)
      return source.Where(e => e.CreatedBy != null && e.CreatedBy.Equals(user));

    return source.Where(_ => false);
  }

  /// <summary>
  /// Applies the filter for the given categories to the given source
  /// </summary>
  /// <param name="source"></param>
  /// <param name="categories"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyCategoriesFilter(IQueryable<SingleEvent> source, HashSet<Category> categories)
  {
    if (categories is null || categories.Count == 0)
      return source;

    // do not turn this into a method group invocation as this will break the code
    return source.Where(e => categories.Contains(e.PrimaryCategory) || e.AdditionalCategories.Any(c => categories.Contains(c)));
  }

  /// <summary>
  /// Applies the filter for the given coordinates and search distance to the given source
  /// </summary>
  /// <param name="source"></param>
  /// <param name="coordinates"></param>
  /// <param name="searchDistanceInMeters"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyLocationFilter(IQueryable<SingleEvent> source, Point? coordinates, double? searchDistanceInMeters)
  {
    if (coordinates is null || searchDistanceInMeters is null)
      return source;

    return source.Where(e => e.EventLocation.Coordinates.IsWithinDistance(coordinates, searchDistanceInMeters.Value));
  }

  /// <summary>
  /// Applies the current date filter to the given source to only match current and future events
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyDateFilter(IQueryable<SingleEvent> source)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);
    return source.Where(e => (e.EndDate != null && e.EndDate >= today) || (e.EndDate == null && e.StartDate >= today));
  }

  /// <summary>
  /// Applies the current date filter to the given source to only match past events
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyPastDateFilter(IQueryable<SingleEvent> source)
  {
    var today = DateOnly.FromDateTime(DateTime.Now);
    return source.Where(e => (e.EndDate != null && e.EndDate < today) || (e.EndDate == null && e.StartDate < today));
  }

  /// <summary>
  /// Applies the filter for the given workflow status to the given source
  /// </summary>
  /// <param name="source"></param>
  /// <param name="workflowStatus"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyWorkflowFilter(IQueryable<SingleEvent> source, WorkflowStatus? workflowStatus)
  {
    if (workflowStatus is null)
      return source;

    return source.Where(e => e.WorkflowStatus == workflowStatus);
  }

  /// <summary>
  /// Applies all necessary includes to the given source
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyIncludes(IQueryable<SingleEvent> source)
  {
    var sourceWithIncludes = source
      .Include(e => e.EventImage)
      .Include(e => e.EventLocation)
      .Include(e => e.CreatedBy)
      .Include(e => e.PrimaryCategory)
      .Include(e => e.AdditionalCategories);

    return sourceWithIncludes;
  }

  /// <summary>
  /// Applies the default ordering to the given source
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  private static IQueryable<SingleEvent> ApplyOrdering(IQueryable<SingleEvent> source)
  {
    var orderedSource = source
      .OrderBy(e => e.StartDate)
      .ThenBy(e => e.StartTime);

    return orderedSource;
  }
}
