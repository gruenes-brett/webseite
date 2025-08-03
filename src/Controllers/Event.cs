using System.Globalization;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.ViewModels.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Sqids;
using Image = GruenesBrett.Models.Image;
using Point = NetTopologySuite.Geometries.Point;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles managing an event
/// </summary>
/// <param name="auditService"></param>
/// <param name="categoryService"></param>
/// <param name="context"></param>
/// <param name="emailService"></param>
/// <param name="eventService"></param>
/// <param name="filterService"></param>
/// <param name="hostEnvironment"></param>
/// <param name="logger"></param>
/// <param name="signInManager"></param>
/// <param name="sqidsEncoder"></param>
/// <param name="textService"></param>
/// <param name="urlService"></param>
/// <param name="userService"></param>
[Route("veranstaltung")]
public class Event(IAuditService auditService, ICategoryService categoryService, ApplicationDbContext context, IEmailService emailService,
  IEventService eventService, IFilterService filterService, IWebHostEnvironment hostEnvironment, ILogger<Event> logger,
  ISettingsService settingsService, SignInManager<ApplicationUser> signInManager, SqidsEncoder<long> sqidsEncoder, ITextService textService,
  IUrlService urlService, IUserService userService) : Controller
{
  /// <summary>
  /// Shows the page with the given external event ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Route("{eventId}")]
  public async Task<IActionResult> Index(string eventId)
  {
    var approvedEvent = await eventService.GetApprovedEventAsync(eventId);
    if (approvedEvent is not null)
      return View(approvedEvent);

    var internalEvent = await eventService.GetEventAsync(eventId, User);
    if (internalEvent is not null)
      return View(internalEvent);

    var eventExists = await eventService.EventExists(eventId);
    if (eventExists)
    {
      var returnUrl = Url.Action(nameof(Index), nameof(Event), new { eventId });
      return RedirectToAction(nameof(Account.Login), nameof(Account), new { returnUrl });
    }

    return RedirectToAction(nameof(Error.Index), nameof(Error), new { statusCode = 404 });
  }

  /// <summary>
  /// Shows the form for creating a new event
  /// </summary>
  /// <returns></returns>
  [Route("anlegen")]
  public IActionResult Create()
  {
    var categories = categoryService.GetAllCategoriesSorted();
    var postCode = filterService.GetCurrentPostCode(HttpContext.Request);
    var longitude = postCode?.Coordinates.X.ToString(NumberFormatInfo.InvariantInfo)
      ?? Constants.Locations.DefaultLongitude.ToString(NumberFormatInfo.InvariantInfo);
    var latitude = postCode?.Coordinates.Y.ToString(NumberFormatInfo.InvariantInfo)
      ?? Constants.Locations.DefaultLatitude.ToString(NumberFormatInfo.InvariantInfo);

    var primaryCategory = categories[0]?.Name ?? string.Empty;
    var viewModel = new CreateViewModel
    {
      CreatedByName = string.Empty,
      CreatedByEmail = string.Empty,
      EventName = string.Empty,
      StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
      LocationName = string.Empty,
      Longitude = longitude,
      Latitude = latitude,
      EventDescription = string.Empty,
      PrimaryCategory = primaryCategory,
      AvailableCategories = new SelectList(categories, "Id", "Name"),
      ConfirmImageRights = false,
      ConfirmPrivacyPolicy = false
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for creating a new event
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("anlegen")]
  public async Task<IActionResult> Create(CreateViewModel viewModel)
  {
    // add the categories again in case of an error
    var categories = categoryService.GetAllCategoriesSorted();
    viewModel.AvailableCategories = new SelectList(categories, "Id", "Name");
    ModelState.Remove("AvailableCategories");

    // check the current user
    var currentUser = await GetCurrentUserAsync();
    if (currentUser is not null)
    {
      ModelState.Remove("CreatedByName");
      ModelState.Remove("CreatedByEmail");
    }

    // check model validations
    var isInvalidModelState = !ModelState.IsValid;
    if (isInvalidModelState)
      return View(viewModel);

    // check custom validations
    var isValid = await IsValid(viewModel);
    if (!isValid)
      return View(viewModel);

    // create the actual event
    var singleEvent = await CreateSingleEventAsync(viewModel, currentUser);
    if (singleEvent is null)
    {
      var createError = textService.GetText(Constants.Text.Event.CreateError);
      ModelState.AddModelError(string.Empty, createError);
      return View(viewModel);
    }

    return RedirectToAction(nameof(CreateConfirmation));
  }

  /// <summary>
  /// Shows the information that an event was created successfully
  /// </summary>
  /// <returns></returns>
  [Route("anlegen-bestaetigung")]
  public IActionResult CreateConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Shows the form for approving the event with the given ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Authorize]
  [Route("freigeben")]
  public async Task<IActionResult> Approve(Guid eventId)
  {
    var draftOrRejectedEvent = await eventService.GetDraftOrRejectedEventAsync(eventId, User);
    if (draftOrRejectedEvent is null)
      return RedirectToAction(nameof(Edit));

    return View(draftOrRejectedEvent);
  }

  /// <summary>
  /// Handles the inputs for the form for approving the event with the given ID
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [Authorize]
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("freigeben")]
  public async Task<IActionResult> Approve(ApproveOrRejectViewModel viewModel)
  {
    var draftOrRejectedEvent = await eventService.GetDraftOrRejectedEventAsync(viewModel.EventId, User);
    if (draftOrRejectedEvent is null)
      return RedirectToAction(nameof(Events.Index), nameof(Events));

    var user = await userService.GetUserAsync(User);
    if (user is null)
      return RedirectToAction(nameof(Events.Index), nameof(Events));

    // if we are approving an edit for an already approved event
    // then we need to delete the already approved version first
    var externalId = draftOrRejectedEvent.ExternalId;
    var approvedEvent = await eventService.GetApprovedEventAsync(externalId);
    if (approvedEvent is not null)
      context.Remove(approvedEvent);

    draftOrRejectedEvent.WorkflowStatus = WorkflowStatus.Approved;
    draftOrRejectedEvent.Updated = DateTime.UtcNow;

    await context.SaveChangesAsync();
    await auditService.LogEventActivityAsync("Event {event} was approved by {email}", externalId, user.Email);

    if (draftOrRejectedEvent.CreatedByEmail.HasValue())
      await SendApprovedEmailAsync(draftOrRejectedEvent.CreatedByEmail, draftOrRejectedEvent.ExternalId);

    return RedirectToAction(nameof(Events.Index), nameof(Events));
  }

  /// <summary>
  /// Shows the form for rejecting the event with the given ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Authorize]
  [Route("ablehnen")]
  public async Task<IActionResult> Reject(Guid eventId)
  {
    var approvedOrDraftEvent = await eventService.GetApprovedOrDraftEventAsync(eventId, User);
    if (approvedOrDraftEvent is null)
      return RedirectToAction(nameof(Edit));

    return View(approvedOrDraftEvent);
  }

  /// <summary>
  /// Handles the inputs for the form for approving the event with the given ID
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [Authorize]
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("ablehnen")]
  public async Task<IActionResult> Reject(ApproveOrRejectViewModel viewModel)
  {
    var approvedOrDraftEvent = await eventService.GetApprovedOrDraftEventAsync(viewModel.EventId, User);
    if (approvedOrDraftEvent is null)
      return RedirectToAction(nameof(Events.Index), nameof(Events));

    var user = await userService.GetUserAsync(User);
    if (user is null)
      return RedirectToAction(nameof(Events.Index), nameof(Events));

    approvedOrDraftEvent.WorkflowStatus = WorkflowStatus.Rejected;
    approvedOrDraftEvent.Updated = DateTime.UtcNow;

    await context.SaveChangesAsync();
    await auditService.LogEventActivityAsync("Event {event} was rejected by {email}", approvedOrDraftEvent.ExternalId, user.Email);

    if (approvedOrDraftEvent.CreatedByEmail.HasValue())
      await SendRejectedEmailAsync(approvedOrDraftEvent.CreatedByEmail, approvedOrDraftEvent.EventName);

    return RedirectToAction(nameof(Events.Index), nameof(Events));
  }

  /// <summary>
  /// Shows the form for editing the event with the given ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Authorize]
  [Route("bearbeiten")]
  public async Task<IActionResult> Edit(Guid eventId)
  {
    var eventToEdit = await eventService.GetEventAsync(eventId, User);
    if (eventToEdit is null)
      return RedirectToAction(nameof(Error.Index), nameof(Error), new { statusCode = 404 });

    var wasInThePast = eventService.EventWasInThePast(eventToEdit);
    if (wasInThePast)
      return RedirectToAction(nameof(Error.Index), nameof(Error), new { statusCode = 404 });

    var categories = categoryService.GetAllCategoriesSorted();
    var viewModel = new EditViewModel
    {
      EventName = eventToEdit.EventName,
      OrganizerName = eventToEdit.OrganizerName,
      EventLink = eventToEdit.EventLink,
      StartDate = eventToEdit.StartDate,
      StartTime = eventToEdit.StartTime,
      EndDate = eventToEdit.EndDate,
      EndTime = eventToEdit.EndTime,
      JoinEveryDay = eventToEdit.JoinEveryDay,
      LocationName = eventToEdit.EventLocation?.Name ?? string.Empty,
      LocationAddress = eventToEdit.EventLocation?.Address,
      Longitude = eventToEdit.EventLocation?.Coordinates?.X.ToString(NumberFormatInfo.InvariantInfo) ?? string.Empty,
      Latitude = eventToEdit.EventLocation?.Coordinates?.Y.ToString(NumberFormatInfo.InvariantInfo) ?? string.Empty,
      EventDescription = eventToEdit.EventDescription,
      AvailableCategories = new SelectList(categories, "Id", "Name"),
      PrimaryCategory = eventToEdit.PrimaryCategory.Id.ToString(),
      AdditionalCategories = eventToEdit.AdditionalCategories?.Select(c => c.Id.ToString()).ToList() ?? [],
      ConfirmImageRights = true,
      ConfirmPrivacyPolicy = true,
      PreviousImage = eventToEdit.EventImage?.FileName,
      EventId = eventId
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for editing the event with the given ID
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [Authorize]
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("bearbeiten")]
  public async Task<IActionResult> Edit(EditViewModel viewModel)
  {
    var eventToEdit = await eventService.GetEventAsync(viewModel.EventId, User);
    if (eventToEdit is null)
      return RedirectToAction(nameof(Error.Index), nameof(Error), new { statusCode = 404 });

    var wasInThePast = eventService.EventWasInThePast(eventToEdit);
    if (wasInThePast)
      return RedirectToAction(nameof(Error.Index), nameof(Error), new { statusCode = 404 });

    // add the categories again in case of an error
    var categories = categoryService.GetAllCategoriesSorted();
    viewModel.AvailableCategories = new SelectList(categories, "Id", "Name");
    ModelState.Remove("AvailableCategories");

    // check model validations
    var isInvalidModelState = !ModelState.IsValid;
    if (isInvalidModelState)
      return View(viewModel);

    // check custom validations
    var isValid = await IsValid(viewModel);
    if (!isValid)
      return View(viewModel);

    // edit the actual event
    var editedEvent = await EditSingleEventAsync(viewModel, eventToEdit);
    if (editedEvent is null)
    {
      var createError = textService.GetText(Constants.Text.Event.EditError);
      ModelState.AddModelError(string.Empty, createError);
      return View(viewModel);
    }

    return RedirectToAction(nameof(Events.Index), nameof(Events));
  }

  /// <summary>
  /// Shows the form for deleting the event with the given ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Authorize]
  [Route("loeschen")]
  public async Task<IActionResult> Delete(Guid eventId)
  {
    var eventToDelete = await eventService.GetEventAsync(eventId, User);
    if (eventToDelete is not null)
      return View(eventToDelete);

    return RedirectToAction(nameof(Events.Index), nameof(Events));
  }

  /// <summary>
  /// Handles the inputs for the form for deleting the event with the given ID
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [Authorize]
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("loeschen")]
  public async Task<IActionResult> Delete(DeleteViewModel viewModel)
  {
    var eventToDelete = await eventService.GetEventAsync(viewModel.EventId, User);
    if (eventToDelete is null)
      return RedirectToAction(nameof(Events.Index), nameof(Events));

    var user = await userService.GetUserAsync(User);
    if (user is null)
      return RedirectToAction(nameof(Events.Index), nameof(Events));

    var externalId = eventToDelete.ExternalId;
    context.Remove(eventToDelete);
    await context.SaveChangesAsync();
    await auditService.LogEventActivityAsync("Event {event} was deleted by {email}", externalId, user.Email);
    return RedirectToAction(nameof(Events.Index), nameof(Events));
  }

  /// <summary>
  /// Shows the form for reporting the event with the given ID
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Route("melden")]
  public IActionResult Report(string eventId)
  {
    var reasons = new List<SelectListItem>
    {
      new(textService.GetText(Constants.Text.Event.ReportDoesNotFitSite), Constants.Text.Event.ReportDoesNotFitSite),
      new(textService.GetText(Constants.Text.Event.ReportIsCanceled), Constants.Text.Event.ReportIsCanceled),
      new(textService.GetText(Constants.Text.Event.ReportIsNotPublic), Constants.Text.Event.ReportIsNotPublic),
      new(textService.GetText(Constants.Text.Event.ReportIsSpam), Constants.Text.Event.ReportIsSpam),
      new(textService.GetText(Constants.Text.Event.ReportIsIllegal), Constants.Text.Event.ReportIsIllegal),
      new(textService.GetText(Constants.Text.Event.ReportIsOther), Constants.Text.Event.ReportIsOther),
    };

    var viewModel = new ReportViewModel
    {
      EventId = eventId,
      Reason = reasons[0].Value,
      Reasons = reasons
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for reporting the event with the given ID
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("melden")]
  public async Task<IActionResult> Report(ReportViewModel viewModel)
  {
    if (!viewModel.Reason.StartsWith(Constants.Text.Event.Report))
      return RedirectToAction(nameof(ReportConfirmation));

    var reason = textService.GetText(viewModel.Reason);
    var reportedEvent = await eventService.GetApprovedEventAsync(viewModel.EventId);
    if (reportedEvent is null)
      return RedirectToAction(nameof(ReportConfirmation));

    if (reportedEvent.LastReported.HasValue && DateTime.UtcNow.Subtract(reportedEvent.LastReported.Value) < TimeSpan.FromDays(1))
      return RedirectToAction(nameof(ReportConfirmation));

    reportedEvent.LastReported = DateTime.UtcNow;
    await context.SaveChangesAsync();

    var relevantUsers = await userService.GetRelevantUsersAsync(reportedEvent);
    if (relevantUsers is null || relevantUsers.Count == 0)
      return RedirectToAction(nameof(ReportConfirmation));

    // send email to all responsible editors and administrators
    foreach (var user in relevantUsers)
      await SendReportingEmailAsync(user.Email!, viewModel.EventId, reason);

    // send email to registered creator of the event
    if (reportedEvent.CreatedBy is not null && !relevantUsers.Contains(reportedEvent.CreatedBy))
      await SendReportingEmailAsync(reportedEvent.CreatedBy.Email!, viewModel.EventId, reason);

    // send email to the unregistered creator of the event
    if (reportedEvent.CreatedByEmail is not null)
      await SendReportingEmailAsync(reportedEvent.CreatedByEmail, viewModel.EventId, reason);

    await auditService.LogEventActivityAsync("Event {event} was reported by someone", viewModel.EventId);
    return RedirectToAction(nameof(ReportConfirmation));
  }

  /// <summary>
  /// Shows the information that the event was reported successfully
  /// </summary>
  /// <returns></returns>
  [Route("melden-bestaetigung")]
  public IActionResult ReportConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Runs custom validation on the given data and returns whether it is valid or not
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  private async Task<bool> IsValid(CreateOrEditViewModel viewModel)
  {
    var isValid = true;

    // check the provided link
    var isValidLink = IsValidLink(viewModel.EventLink);
    if (!isValidLink)
    {
      var invalidLinkError = textService.GetText(Constants.Text.Event.InvalidLink);
      ModelState.AddModelError(nameof(viewModel.EventLink), invalidLinkError);
      isValid = false;
    }

    // check the provided duration
    var (isTooLong, maximumEventLength) = await IsTooLong(viewModel.StartDate, viewModel.EndDate);
    if (isTooLong)
    {
      var invalidLengthError = textService.GetText(Constants.Text.Event.InvalidEventLength);
      var formettedInvalidLengthError = invalidLengthError.FormatWith(maximumEventLength);
      ModelState.AddModelError(nameof(viewModel.EndDate), formettedInvalidLengthError);
      isValid = false;
    }

    // check the start date
    var startDateInThePast = viewModel.StartDate.IsInThePast();
    if (startDateInThePast)
    {
      var invalidDateError = textService.GetText(Constants.Text.Event.InvalidDate);
      ModelState.AddModelError(nameof(viewModel.StartDate), invalidDateError);
      isValid = false;
    }

    // check the end date
    var endDateInThePast = viewModel.EndDate?.IsInThePast() == true;
    if (endDateInThePast)
    {
      var invalidDateError = textService.GetText(Constants.Text.Event.InvalidDate);
      ModelState.AddModelError(nameof(viewModel.EndDate), invalidDateError);
      isValid = false;
    }

    // check the start and end date
    var endDateBeforeStartDate = viewModel.EndDate is not null && viewModel.EndDate < viewModel.StartDate;
    if (endDateBeforeStartDate)
    {
      var invalidDatesError = textService.GetText(Constants.Text.Event.InvalidDates);
      ModelState.AddModelError(nameof(viewModel.EndDate), invalidDatesError);
      isValid = false;
    }

    // check the start time
    var startTimeIsMissing = viewModel.EndTime is not null && viewModel.StartTime is null;
    if (startTimeIsMissing)
    {
      var invalidStartTimeError = textService.GetText(Constants.Text.Event.InvalidStartTime);
      ModelState.AddModelError(nameof(viewModel.StartTime), invalidStartTimeError);
      isValid = false;
    }

    // check the end time
    var isSingleDay = viewModel.EndDate is null || viewModel.EndDate == viewModel.StartDate;
    var hasBothTimes = viewModel.StartTime is not null && viewModel.EndTime is not null;
    var endTimeBeforeStartTime = viewModel.EndTime < viewModel.StartTime;
    if (isSingleDay && hasBothTimes && endTimeBeforeStartTime)
    {
      var invalidEndTimeError = textService.GetText(Constants.Text.Event.InvalidEndTime);
      ModelState.AddModelError(nameof(viewModel.EndTime), invalidEndTimeError);
      isValid = false;
    }

    // check the provided image
    var imageIsAllowedToUpload = IsAllowedToUpload(viewModel.EventImage);
    if (!imageIsAllowedToUpload)
    {
      var invalidImageError = textService.GetText(Constants.Text.Event.InvalidImage);
      ModelState.AddModelError(nameof(viewModel.EventImage), invalidImageError);
      isValid = false;
    }

    // check the confirmation of image rights
    var imageRightsConfirmed = viewModel.EventImage is null || viewModel.ConfirmImageRights;
    if (!imageRightsConfirmed)
    {
      var invalidImageRightsError = textService.GetText(Constants.Text.Event.InvalidImageRights);
      ModelState.AddModelError(nameof(viewModel.ConfirmImageRights), invalidImageRightsError);
      isValid = false;
    }

    return isValid;
  }

  /// <summary>
  /// Returns the current user if one is logged in
  /// </summary>
  /// <returns></returns>
  private async Task<ApplicationUser?> GetCurrentUserAsync()
  {
    var isSignedIn = signInManager.IsSignedIn(User);
    if (!isSignedIn)
      return null;

    return await userService.GetUserAsync(User);
  }

  /// <summary>
  /// Creates a new event based on the given information
  /// </summary>
  /// <param name="viewModel"></param>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task<SingleEvent?> CreateSingleEventAsync(CreateViewModel viewModel, ApplicationUser? user)
  {
    var eventLocation = GetLocation(viewModel);
    if (eventLocation is null)
      return null;

    var primaryCategory = GetCategory(viewModel.PrimaryCategory);
    if (primaryCategory is null)
      return null;

    var additionalCategories = GetAdditionalCategories(viewModel.AdditionalCategories, primaryCategory);
    var eventImage = await CreateImageAsync(viewModel, user);

    var internalId = Guid.CreateVersion7();
    var externalId = sqidsEncoder.Encode(DateTime.UtcNow.Ticks);
    var created = DateTime.UtcNow;
    var updated = DateTime.UtcNow;
    var createdByName = user is null ? viewModel.CreatedByName : string.Empty;
    var createdByEmail = user is null ? viewModel.CreatedByEmail : string.Empty;
    var eventName = viewModel.EventName.Trim();
    var organizerName = viewModel.OrganizerName?.Trim();
    var eventLink = viewModel.EventLink?.Trim();
    var eventDescription = viewModel.EventDescription.Trim();
    var workflowStatus = user is null ? WorkflowStatus.Draft : WorkflowStatus.Approved;

    var singleEvent = new SingleEvent
    {
      InternalId = internalId,
      ExternalId = externalId,
      Created = created,
      Updated = updated,
      CreatedBy = user,
      CreatedByName = createdByName,
      CreatedByEmail = createdByEmail,
      EventName = eventName,
      OrganizerName = organizerName,
      EventLink = eventLink,
      StartDate = viewModel.StartDate,
      EndDate = viewModel.EndDate,
      StartTime = viewModel.StartTime,
      EndTime = viewModel.EndTime,
      JoinEveryDay = viewModel.JoinEveryDay,
      EventLocation = eventLocation,
      EventDescription = eventDescription,
      PrimaryCategory = primaryCategory,
      AdditionalCategories = additionalCategories,
      EventImage = eventImage,
      WorkflowStatus = workflowStatus
    };

    context.Categories.Attach(singleEvent.PrimaryCategory);
    context.Categories.AttachRange(singleEvent.AdditionalCategories);
    context.SingleEvents.Add(singleEvent);
    await context.SaveChangesAsync();

    if (user is null)
    {
      var email = createdByEmail;
      await auditService.LogEventActivityAsync("Event {event} was created by anonymous user {email} and is waiting for approval", externalId, email);
      await SendApprovalEmailAsync(singleEvent);
    }
    else
    {
      var email = user.Email;
      await auditService.LogEventActivityAsync("Event {event} was created by user {email} and automatically approved", externalId, email);
    }

    return singleEvent;
  }

  /// <summary>
  /// Edits the given event based on the given information
  /// </summary>
  /// <param name="viewModel"></param>
  /// <param name="eventToEdit"></param>
  /// <returns></returns>
  private async Task<SingleEvent?> EditSingleEventAsync(EditViewModel viewModel, SingleEvent eventToEdit)
  {
    var user = await userService.GetUserAsync(User);
    if (user is null)
      return null;

    eventToEdit.Updated = DateTime.UtcNow;
    eventToEdit.EventName = viewModel.EventName.Trim();
    eventToEdit.OrganizerName = viewModel.OrganizerName?.Trim();
    eventToEdit.EventLink = viewModel.EventLink?.Trim();
    eventToEdit.StartDate = viewModel.StartDate;
    eventToEdit.StartTime = viewModel.StartTime;
    eventToEdit.EndDate = viewModel.EndDate;
    eventToEdit.EndTime = viewModel.EndTime;
    eventToEdit.JoinEveryDay = viewModel.JoinEveryDay;
    eventToEdit.EventDescription = viewModel.EventDescription.Trim();

    var viewModelLocation = GetLocation(viewModel);
    var isSameLocation = IsSameLocation(viewModelLocation, eventToEdit.EventLocation);
    if (viewModelLocation is not null && !isSameLocation)
    {
      var oldLocation = eventToEdit.EventLocation;
      context.Add(viewModelLocation);
      eventToEdit.EventLocation = null!;
      eventToEdit.EventLocation = viewModelLocation;
      context.Remove(oldLocation);
    }

    eventToEdit.PrimaryCategory = GetPrimaryCategory(viewModel.PrimaryCategory, eventToEdit.PrimaryCategory);
    eventToEdit.AdditionalCategories = GetAdditionalCategories(viewModel.AdditionalCategories, eventToEdit.PrimaryCategory);

    var eventImage = await CreateImageAsync(viewModel, user);
    if (eventImage is not null)
    {
      var oldImage = eventToEdit.EventImage;
      context.Add(eventImage);
      eventToEdit.EventImage = null;
      eventToEdit.EventImage = eventImage;
      if (oldImage is not null)
        context.Remove(oldImage);
    }

    await context.SaveChangesAsync();
    await auditService.LogEventActivityAsync("Event {event} was edited by {email}", eventToEdit.ExternalId, user.Email);
    return eventToEdit;
  }

  /// <summary>
  /// Creates a new location from the given view model and returns it
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  private static Location? GetLocation(CreateOrEditViewModel viewModel)
  {
    var hasLongitude = double.TryParse(viewModel.Longitude, NumberFormatInfo.InvariantInfo, out var longitude);
    var hasLatitude = double.TryParse(viewModel.Latitude, NumberFormatInfo.InvariantInfo, out var latitude);

    if (!hasLongitude || !hasLatitude)
      return null;

    var id = Guid.CreateVersion7();
    var name = viewModel.LocationName.Trim();
    var address = viewModel.LocationAddress?.Trim();
    var coordinates = new Point(longitude, latitude) { SRID = Constants.Locations.Srid };
    return new Location
    {
      Id = id,
      Name = name,
      Address = address,
      Coordinates = coordinates
    };
  }

  /// <summary>
  /// Returns whether two given locations are the same or not
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <returns></returns>
  private static bool IsSameLocation(Location? a, Location? b)
  {
    if (a is null && b is null)
      return true;

    if (a is null || b is null)
      return false;

    var hasSameName = a.Name == b.Name;
    var hasSameAddress = a.Address == b.Address;
    var hasSameCoordinates = a.Coordinates == b.Coordinates;
    return hasSameName && hasSameAddress && hasSameCoordinates;
  }

  /// <summary>
  /// Returns the category for the given ID
  /// </summary>
  /// <param name="categoryId"></param>
  /// <returns></returns>
  private Category? GetCategory(string? categoryId)
  {
    if (categoryId.IsNullOrEmpty())
      return null;

    if (!Guid.TryParse(categoryId, out var guid))
      return null;

    return context.Categories.Find(guid);
  }

  /// <summary>
  /// Returns the category for the given ID
  /// or the given fallback category if it does not exist
  /// </summary>
  /// <param name="categoryId"></param>
  /// <param name="fallbackCategory"></param>
  /// <returns></returns>
  private Category GetPrimaryCategory(string? categoryId, Category fallbackCategory)
  {
    var primaryCategory = GetCategory(categoryId);
    return primaryCategory ?? fallbackCategory;
  }

  /// <summary>
  /// Returns the categories for the given IDs, except for the given primary category
  /// </summary>
  /// <param name="categoryIds"></param>
  /// <param name="primaryCategory"></param>
  /// <returns></returns>
  private List<Category> GetAdditionalCategories(List<string>? categoryIds, Category primaryCategory)
  {
    if (categoryIds is null)
      return [];

    if (categoryIds.Count == 0)
      return [];

    var additionalCategories = new List<Category>();
    foreach (var categoryId in categoryIds)
    {
      var category = GetCategory(categoryId);
      if (category is not null)
        additionalCategories.Add(category);
    }

    additionalCategories.Remove(primaryCategory);
    return additionalCategories;
  }

  /// <summary>
  /// Returns whether the given link is a valid URI or not
  /// </summary>
  /// <param name="link"></param>
  /// <returns></returns>
  private static bool IsValidLink(string? link)
  {
    if (link.IsNullOrEmpty())
      return true;

    return Uri.TryCreate(link, UriKind.Absolute, out var _);
  }

  /// <summary>
  /// Returns whether the given dates are too long apart or not
  /// </summary>
  /// <param name="startDate"></param>
  /// <param name="endDate"></param>
  /// <returns></returns>
  private async Task<(bool, int)> IsTooLong(DateOnly startDate, DateOnly? endDate)
  {
    if (endDate is null)
      return (false, 0);

    var maximumEventLength = await settingsService.GetIntSettingAsync(Constants.Settings.MaximumEventLength);
    var eventLength = endDate.Value.DayNumber - startDate.DayNumber;
    return (eventLength > maximumEventLength, maximumEventLength);
  }

  /// <summary>
  /// Returns whether the given image file is allowed to be uploaded to the site or not
  /// </summary>
  /// <param name="formFile"></param>
  /// <returns></returns>
  private static bool IsAllowedToUpload(IFormFile? formFile)
  {
    if (formFile is null)
      return true;

    if (formFile.Length < 0 || formFile.Length > Constants.Image.MaxSize)
      return false;

    if (formFile.ContentType != Constants.Image.Jpeg && formFile.ContentType != Constants.Image.Png)
      return false;

    return true;
  }

  /// <summary>
  /// Resizes the uploaded image, writes it to the file system
  /// and returns information about the resulting file
  /// </summary>
  /// <param name="viewModel"></param>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task<Image?> CreateImageAsync(CreateOrEditViewModel viewModel, ApplicationUser? user)
  {
    if (viewModel.EventImage is null)
      return null;

    var id = Guid.CreateVersion7();
    var fileExtension = viewModel.EventImage.ContentType switch
    {
      Constants.Image.Png => ".png",
      _ => ".jpg"
    };
    var fileName = $"{id:D}{fileExtension}";
    var rootPath = hostEnvironment.WebRootPath;
    const string uploadFolder = Constants.System.UploadFolder;
    var dateFolder = $"{DateTime.Now.Year}-{DateTime.Now.Month:D2}";
    var relativeFilePath = Path.Combine(uploadFolder, dateFolder, fileName);
    var absoluteFilePath = Path.Combine(rootPath, relativeFilePath);
    var absoluteFolderPath = Path.Combine(rootPath, uploadFolder, dateFolder);
    var relativeUri = relativeFilePath.Replace('\\', '/');

    Directory.CreateDirectory(absoluteFolderPath);
    await using (var stream = System.IO.File.Create(absoluteFilePath))
      await viewModel.EventImage.CopyToAsync(stream);

    var (width, height) = await CropImageToSizeAsync(absoluteFilePath, viewModel);
    return new Image
    {
      Id = id,
      FileName = relativeUri,
      UploadedBy = user,
      Width = width,
      Height = height
    };
  }

  /// <summary>
  /// Crops the image with the given file path to the given dimensions,
  /// overrides the original file and returns the resized dimensions
  /// </summary>
  /// <param name="filePath"></param>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  private static async Task<(int Width, int Height)> CropImageToSizeAsync(string filePath, CreateOrEditViewModel viewModel)
  {
    var top = viewModel.EventImageCropTop;
    var left = viewModel.EventImageCropLeft;
    var width = viewModel.EventImageCropWidth;
    var height = (int)Math.Floor(width / 16.0 * 9.0);
    var cropRectangle = new Rectangle(left, top, width, height);

    using SixLabors.ImageSharp.Image image = await SixLabors.ImageSharp.Image.LoadAsync(filePath);

    image.Mutate(x => x.AutoOrient());
    image.Mutate(x => x.Crop(cropRectangle));

    if (width > Constants.Image.MaxWidth)
      image.Mutate(x => x.Resize(Constants.Image.MaxWidth, 0));

    await image.SaveAsync(filePath);
    return (image.Width, image.Height);
  }

  /// <summary>
  /// Sends the email for approving the given event to all relevant editors
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  private async Task SendApprovalEmailAsync(SingleEvent singleEvent)
  {
    try
    {
      var relevantUsers = await userService.GetRelevantUsersAsync(singleEvent);
      if (relevantUsers.Count > 0)
      {
        var eventId = singleEvent.ExternalId;
        var eventApprovalLink = urlService.GetAbsoluteUrl(nameof(Event), nameof(Index), new { eventId });
        foreach (var user in relevantUsers)
          await SendApprovalEmailAsync(user, eventApprovalLink);
      }
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send email about new event {eventId}", singleEvent.ExternalId);
    }
  }

  /// <summary>
  /// Sends the email that the event for the given ID was approved to the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <param name="eventId"></param>
  /// <returns></returns>
  private async Task SendApprovedEmailAsync(string email, string eventId)
  {
    try
    {
      var approvedEventLink = urlService.GetAbsoluteUrl(nameof(Event), nameof(Index), new { eventId });

      var subject = textService.GetText(Constants.Text.Event.ApprovedEmailSubject);
      var body = textService.GetText(Constants.Text.Event.ApprovedEmailBody);
      body = string.Format(body, approvedEventLink);

      await emailService.SendEmailAsync(email, subject, body, EmailType.Optional);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send email about approved event to user {email}", email);
    }
  }

  /// <summary>
  /// Sends the email that the event for the given ID was rejected to the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <param name="eventName"></param>
  /// <returns></returns>
  private async Task SendRejectedEmailAsync(string email, string eventName)
  {
    try
    {
      var subject = textService.GetText(Constants.Text.Event.RejectedEmailSubject);
      var body = textService.GetText(Constants.Text.Event.RejectedEmailBody);
      body = string.Format(body, eventName);

      await emailService.SendEmailAsync(email, subject, body, EmailType.Optional);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send email about rejected event to user {email}", email);
    }
  }

  /// <summary>
  /// Sends the email with a link to approve a new event to the given editor
  /// </summary>
  /// <param name="user"></param>
  /// <param name="eventApprovalLink"></param>
  /// <returns></returns>
  private async Task SendApprovalEmailAsync(ApplicationUser user, string eventApprovalLink)
  {
    try
    {
      var subject = textService.GetText(Constants.Text.Event.ApprovalEmailSubject);
      var body = textService.GetText(Constants.Text.Event.ApprovalEmailBody);
      body = string.Format(body, eventApprovalLink);

      await emailService.SendEmailAsync(user.Email!, subject, body, EmailType.Optional);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send email about new event to user {email}", user.Email);
    }
  }

  /// <summary>
  /// Sends the email that the event for the given ID was reported
  /// for the given reason to the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <param name="eventId"></param>
  /// <param name="reason"></param>
  /// <returns></returns>
  private async Task SendReportingEmailAsync(string email, string eventId, string reason)
  {
    try
    {
      var reportedEventLink = urlService.GetAbsoluteUrl(nameof(Event), nameof(Index), new { eventId });
      var subject = textService.GetText(Constants.Text.Event.ReportEmailSubject);
      var body = textService.GetText(Constants.Text.Event.ReportEmailBody);
      body = string.Format(body, reportedEventLink, reason);

      await emailService.SendEmailAsync(email, subject, body, EmailType.Reporting);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send email about reported event to user {email}", email);
    }
  }
}
