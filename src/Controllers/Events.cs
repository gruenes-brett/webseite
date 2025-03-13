using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.ViewModels.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles showing collections of events
/// </summary>
/// <param name="auditService"></param>
/// <param name="categoryService"></param>
/// <param name="eventService"></param>
/// <param name="filterService"></param>
/// <param name="userService"></param>
[Route("veranstaltungen")]
public class Events(IAuditService auditService, ICategoryService categoryService, IEventService eventService,
  IFilterService filterService, IUserService userService) : Controller
{
  /// <summary>
  /// Shows the explore page
  /// </summary>
  /// <returns></returns>
  [Route("erkunden")]
  public async Task<IActionResult> Explore()
  {
    var filterViewModel = filterService.GetFilterViewModel(HttpContext.Request, $"{nameof(Events)}/{nameof(Explore)}");
    if (filterViewModel?.PostCode is null)
      return Redirect("/");

    var categories = filterViewModel.SelectedCategories;
    var coordinates = filterViewModel.PostCode.Coordinates;
    var searchDistanceInMeters = filterViewModel.SearchDistanceInMeters;
    var events = await eventService.GetApprovedEventsAsync(categories, coordinates, searchDistanceInMeters);
    var exploreViewModel = new ExploreViewModel
    {
      Events = events ?? [],
      FilterViewModel = filterViewModel
    };
    return View(exploreViewModel);
  }

  /// <summary>
  /// Shows the calendar page
  /// </summary>
  /// <returns></returns>
  [Route("kalender")]
  public async Task<IActionResult> Calendar()
  {
    var filterViewModel = filterService.GetFilterViewModel(HttpContext.Request, $"{nameof(Events)}/{nameof(Calendar)}");
    if (filterViewModel.PostCode is null)
      return Redirect("/");

    var categories = filterViewModel.SelectedCategories;
    var coordinates = filterViewModel.PostCode.Coordinates;
    var searchDistanceInMeters = filterViewModel.SearchDistanceInMeters;
    var events = await eventService.GetApprovedEventsAsync(categories, coordinates, searchDistanceInMeters);
    var dateEvents = GetDateEvents(events);
    var viewModel = new CalendarViewModel
    {
      DateEvents = dateEvents ?? [],
      FilterViewModel = filterViewModel
    };
    return View(viewModel);
  }

  /// <summary>
  /// Shows the map page
  /// </summary>
  /// <returns></returns>
  [Route("karte")]
  public IActionResult Map()
  {
    return View();
  }

  /// <summary>
  /// Shows the list of relevant events for logged in users
  /// </summary>
  /// <returns></returns>
  [Authorize]
  [Route("")]
  public async Task<IActionResult> Index()
  {
    var user = await userService.GetUserAsync(User);
    if (user is null)
      return RedirectToAction(nameof(Explore));

    var viewModel = GetEditViewModel();
    viewModel.Events = await GetRelevantEvents(viewModel.EventStatus, viewModel.SelectedCategories);
    return View(viewModel);
  }

  /// <summary>
  /// Shows the log with all event related activities
  /// </summary>
  /// <returns></returns>
  [Authorize(Roles = Constants.Roles.Administrator)]
  [Route("aktivitaeten-ansehen")]
  public async Task<IActionResult> ShowAuditLog()
  {
    var auditEntries = await auditService.GetNewestActivities(Constants.Audit.Event);
    return View(auditEntries);
  }

  /// <summary>
  /// Reads the currently selected filters for editing events from the cookies and returns them
  /// </summary>
  /// <returns></returns>
  private EditViewModel GetEditViewModel()
  {
    var allCategories = categoryService.GetAllCategoriesSorted();
    var selectedCategories = filterService.GetCurrentSelectedCategories(HttpContext.Request);
    var eventStatus = filterService.GetCurrentEventStatus(HttpContext.Request);

    return new EditViewModel
    {
      Events = [],
      Categories = allCategories ?? [],
      SelectedCategories = selectedCategories ?? [],
      EventStatus = eventStatus
    };
  }

  /// <summary>
  /// Maps the given list of events to a dictionary of days with their corresponding events
  /// </summary>
  /// <param name="events"></param>
  /// <returns></returns>
  private static OrderedDictionary<DateOnly, List<SingleEvent>> GetDateEvents(List<SingleEvent> events)
  {
    if (events.Count == 0)
      return [];

    var dateEvents = new OrderedDictionary<DateOnly, List<SingleEvent>>();
    var today = DateOnly.FromDateTime(DateTime.Now);
    var lastDate = events.Max(e => e.RelevantDate);
    for (var day = today; day <= lastDate; day = day.AddDays(1))
      dateEvents.Add(day, []);

    foreach (var singleEvent in events)
    {
      var endDate = singleEvent.RelevantDate;
      for (var day = singleEvent.StartDate; day <= endDate; day = day.AddDays(1))
      {
        if (!singleEvent.JoinEveryDay && day != singleEvent.StartDate)
          continue;

        if (dateEvents.TryGetValue(day, out var eventsOnThisDay))
        {
          eventsOnThisDay.Add(singleEvent);
          dateEvents[day] = [.. eventsOnThisDay.OrderBy(e => e.StartTime)];
        }
      }
    }

    return dateEvents;
  }

  /// <summary>
  /// Returns all relevant for the given (combined) workflow status and list of categories
  /// </summary>
  /// <param name="status"></param>
  /// <param name="categories"></param>
  /// <returns></returns>
  private async Task<List<SingleEvent>> GetRelevantEvents(string status, HashSet<Category> categories)
  {
    var events = new List<SingleEvent>();

    if (status == Constants.Status.AllStatuses || status == Constants.Status.Approved)
    {
      var approvedEvents = await eventService.GetApprovedEventsAsync(categories, principal: User);
      events.AddRange(approvedEvents);
    }

    if (status == Constants.Status.AllStatuses || status == Constants.Status.WaitingForApproval)
    {
      var drafts = await eventService.GetDraftEventsAsync(categories, User);
      events.AddRange(drafts);
    }

    if (status == Constants.Status.AllStatuses || status == Constants.Status.Rejected)
    {
      var rejectedEvents = await eventService.GetRejectedEventsAsync(categories, User);
      events.AddRange(rejectedEvents);
    }

    return [.. events.OrderByDescending(e => e.Created)];
  }
}
