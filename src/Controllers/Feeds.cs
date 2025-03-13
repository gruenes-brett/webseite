using System.Security;
using System.Text;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.Properties;
using GruenesBrett.ViewModels.Feeds;
using Microsoft.AspNetCore.Mvc;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles feeds
/// </summary>
[Route("feeds")]
public class Feeds(ICategoryService categoryService, IEventService eventService, IFilterService filterService,
  ILocationService locationService, IPostCodeService postCodeService, ITextService textService, IUrlService urlService) : Controller
{
  /// <summary>
  /// Shows the page with the various feeds
  /// </summary>
  /// <returns></returns>
  [Route("")]
  public IActionResult Index()
  {
    var filterViewModel = filterService.GetFilterViewModel(HttpContext.Request, $"{nameof(Feeds)}/{nameof(Index)}");
    if (filterViewModel?.PostCode is null)
      return Redirect("/");

    var icalRssViewModel = new IcalAtomViewModel
    {
      SelectedCategories = filterViewModel.SelectedCategories.Select(c => c.Id),
      PostCode = filterViewModel.PostCode.Id,
      SearchDistance = filterViewModel.SearchDistance,
    };

    var atomUrl = urlService.GetAbsoluteUrl(nameof(Feeds), nameof(AtomEvents), icalRssViewModel);
    var icalUrl = urlService.GetAbsoluteUrl(nameof(Feeds), nameof(IcalEvents), icalRssViewModel);

    var feedsViewModel = new FeedsViewModel
    {
      ApiUrl = Constants.System.ApiReference,
      AtomUrl = atomUrl,
      IcalUrl = icalUrl,
      FilterViewModel = filterViewModel
    };
    return View(feedsViewModel);
  }

  /// <summary>
  /// Returns the Atom feed for all events matching the given parameters
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [Route("atom/events")]
  public async Task<IActionResult> AtomEvents(IcalAtomViewModel viewModel)
  {
    var events = await GetEventsAsync(viewModel);
    var selfLink = urlService.GetAbsoluteUrl(nameof(Feeds), nameof(Feeds.AtomEvents), viewModel);
    var atomEvents = GetAtomEvents(events, selfLink);

    return Content(atomEvents, "application/atom+xml");
  }

  /// <summary>
  /// Returns the iCal for a single events
  /// </summary>
  /// <param name="eventId"></param>
  /// <returns></returns>
  [Route("ical/event/{eventId}")]
  public async Task<IActionResult> IcalEvent(string eventId)
  {
    var singleEvent = await eventService.GetApprovedEventAsync(eventId);
    if (singleEvent is null)
      return RedirectToAction(nameof(Error.Index), nameof(Error), new { statusCode = 404 });

    var icalEvent = GetIcalEvent(singleEvent);
    return Content(icalEvent, "text/calendar");
  }

  /// <summary>
  /// Returns the iCal for all events matching the given parameters
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [Route("ical/events")]
  public async Task<IActionResult> IcalEvents(IcalAtomViewModel viewModel)
  {
    var events = await GetEventsAsync(viewModel);
    var icalEvents = GetIcalEvents(events);
    return Content(icalEvents, "text/calendar");
  }

  /// <summary>
  /// Returns the XML sitemap
  /// </summary>
  /// <returns></returns>
  [Route("/sitemap.xml")]
  public async Task<IActionResult> Sitemap()
  {
    var events = await eventService.GetApprovedEventsAsync([]);
    var sitemapEvents = GetSitemapEventsAndPages(events);
    return Content(sitemapEvents, "application/xml");
  }

  /// <summary>
  /// Returns the robots TXT
  /// </summary>
  /// <returns></returns>
  [Route("/robots.txt")]
  public IActionResult Robots()
  {
    return Content("Sitemap: /sitemap.xml", "text/plain");
  }

  /// <summary>
  /// Returns the events for the given parameters
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  private async Task<List<SingleEvent>> GetEventsAsync(IcalAtomViewModel viewModel)
  {
    if (viewModel.SelectedCategories is null)
      return [];

    var categories = categoryService.GetCategories([.. viewModel.SelectedCategories]);
    if (categories.Count == 0)
      return [];

    if (viewModel.PostCode is null)
      return [];

    var postCode = postCodeService.GetPostCode(viewModel.PostCode);
    if (postCode == null)
      return [];

    var coordinates = postCode.Coordinates;
    var searchDistanceInMeters = locationService.GetSearchDistanceInMeters(viewModel.SearchDistance);
    if (searchDistanceInMeters == 0)
      return [];

    var events = await eventService.GetApprovedEventsAsync(categories, coordinates, searchDistanceInMeters);
    return events;
  }

  /// <summary>
  /// Returns the given events formatted as an Atom feed
  /// </summary>
  /// <param name="singleEvents"></param>
  /// <param name="selfLink"></param>
  /// <returns></returns>
  private string GetAtomEvents(List<SingleEvent> singleEvents, string selfLink)
  {
    var title = SystemTexts.SiteName;
    var homepageLink = urlService.GetAbsoluteUrl(nameof(Homepage), nameof(Homepage.Index));
    var updated = singleEvents.Count > 0 ? singleEvents.Max(singleEvent => singleEvent.Updated) : DateTime.UtcNow;
    var escapedSelfLink = SecurityElement.Escape(selfLink);

    var result = new StringBuilder();

    result.AppendLine("""<?xml version="1.0" encoding="utf-8"?>""");
    result.AppendLine("""<feed xmlns="http://www.w3.org/2005/Atom">""");
    result.AppendLine($"<title>{title}</title>");
    result.AppendLine($"""<link href="{escapedSelfLink}" rel="self" />""");
    result.AppendLine($"""<link href="{homepageLink}" />""");
    result.AppendLine("<id>urn:publicid:gruenesbrett</id>");
    result.AppendLine($"<updated>{updated:O}</updated>");

    foreach (var singleEvent in singleEvents)
    {
      var permalink = urlService.GetPermalink(singleEvent);
      var escapedTitle = SecurityElement.Escape(singleEvent.EventName);
      var escapedDescription = SecurityElement.Escape(singleEvent.EventDescription);

      result.AppendLine("<entry>");
      result.AppendLine($"<title>{escapedTitle}</title>");
      result.AppendLine($"""<link href="{permalink}" />""");
      result.AppendLine($"<id>urn:publicid:{singleEvent.ExternalId}</id>");
      result.AppendLine($"<updated>{singleEvent.Updated:O}</updated>");
      result.AppendLine($"""<content type="text">{escapedDescription}</content>""");

      if (singleEvent.OrganizerName.HasValue())
      {
        var escapedOrganizerName = SecurityElement.Escape(singleEvent.OrganizerName);
        result.AppendLine($"<author><name>{escapedOrganizerName}</name></author>");
      }

      result.AppendLine("</entry>");
    }

    result.AppendLine("</feed>");

    return result.ToString();
  }

  /// <summary>
  /// Returns the given event formatted as an iCal calendar
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <returns></returns>
  private string GetIcalEvent(SingleEvent singleEvent)
  {
    var stringBuilder = new StringBuilder();

    AppendCalendarStart(stringBuilder);
    AppendEvent(singleEvent, stringBuilder);
    AppendCalendarEnd(stringBuilder);

    return stringBuilder.ToString();
  }

  /// <summary>
  /// Returns the given events formatted as an iCal calendar
  /// </summary>
  /// <param name="singleEvents"></param>
  /// <returns></returns>
  private string GetIcalEvents(List<SingleEvent> singleEvents)
  {
    var stringBuilder = new StringBuilder();

    AppendCalendarStart(stringBuilder);
    foreach (var singleEvent in singleEvents)
      AppendEvent(singleEvent, stringBuilder);
    AppendCalendarEnd(stringBuilder);

    return stringBuilder.ToString();
  }

  /// <summary>
  /// Appends the start lines of an iCal calendar to the given string builder
  /// </summary>
  /// <param name="stringBuilder"></param>
  private static void AppendCalendarStart(StringBuilder stringBuilder)
  {
    var title = SystemTexts.SiteName;

    stringBuilder.AppendLine("BEGIN:VCALENDAR");
    stringBuilder.AppendLine("VERSION:2.0");
    stringBuilder.AppendLine("PRODID:-//gruenesbrett//NONSGML v1.0//EN");
    stringBuilder.AppendLine($"X-WR-CALNAME:{title}");
  }

  /// <summary>
  /// Appends the end line of an iCal calendar to the given string builder
  /// </summary>
  /// <param name="stringBuilder"></param>
  private static void AppendCalendarEnd(StringBuilder stringBuilder)
  {
    stringBuilder.AppendLine("END:VCALENDAR");
  }

  /// <summary>
  /// Appends the lines for an iCal entry for the given event to the given string builder
  /// </summary>
  /// <param name="singleEvent"></param>
  /// <param name="stringBuilder"></param>
  private void AppendEvent(SingleEvent singleEvent, StringBuilder stringBuilder)
  {
    var start = textService.GetFormattedStartDateAndTime(singleEvent);
    var end = textService.GetFormattedEndDateAndTime(singleEvent);
    var escapedDescription = singleEvent.EventDescription.Replace('\n', ' ').Replace('\r', ' ');
    var categories = singleEvent.PrimaryCategory.Name;
    if (singleEvent.AdditionalCategories.Count > 0)
    {
      var additionalCategories = singleEvent.AdditionalCategories.Select(c => c.Name);
      var joinedAdditionalCategories = string.Join(',', additionalCategories);
      categories += $",{joinedAdditionalCategories}";
    }
    var permalink = urlService.GetPermalink(singleEvent);

    stringBuilder.AppendLine("BEGIN:VEVENT");
    stringBuilder.AppendLine($"UID:{singleEvent.ExternalId}@gruenesbrett");
    stringBuilder.AppendLine($"DTSTAMP:{singleEvent.Updated:yyyyMMddTHHmmssZ}");
    stringBuilder.AppendLine($"DTSTART:{start}");

    if (end.HasValue())
      stringBuilder.AppendLine($"DTEND:{end}");

    stringBuilder.AppendLine($"SUMMARY:{singleEvent.EventName}");
    stringBuilder.AppendLine($"DESCRIPTION:{escapedDescription}");
    stringBuilder.AppendLine($"CATEGORIES:{categories}");
    stringBuilder.AppendLine($"URL:{permalink}");
    stringBuilder.AppendLine($"LOCATION:{singleEvent.EventLocation.Name}");
    stringBuilder.AppendLine("END:VEVENT");
  }

  /// <summary>
  /// Returns the given events formatted as an XML sitemap
  /// </summary>
  /// <param name="singleEvents"></param>
  /// <returns></returns>
  private string GetSitemapEventsAndPages(List<SingleEvent> singleEvents)
  {
    var stringBuilder = new StringBuilder();

    stringBuilder.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
    stringBuilder.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

    var pages = GetSitemapPages();
    foreach (var page in pages)
    {
      stringBuilder.AppendLine("<url>");
      stringBuilder.AppendLine($"<loc>{page}</loc>");
      stringBuilder.AppendLine("</url>");
    }

    foreach (var singleEvent in singleEvents)
    {
      var permalink = urlService.GetPermalink(singleEvent);
      var updated = singleEvent.Updated.ToString("yyyy-MM-dd");

      stringBuilder.AppendLine("<url>");
      stringBuilder.AppendLine($"<loc>{permalink}</loc>");
      stringBuilder.AppendLine($"<lastmod>{updated}</lastmod>");
      stringBuilder.AppendLine("</url>");
    }

    stringBuilder.AppendLine("</urlset>");

    return stringBuilder.ToString();
  }

  /// <summary>
  /// Returns a list of permalinks to mostly static pages
  /// that should be included in the XML sitemap
  /// </summary>
  /// <returns></returns>
  private IEnumerable<string> GetSitemapPages()
  {
    var homepage = urlService.GetAbsoluteUrl(nameof(Homepage), nameof(Homepage.Index));
    yield return homepage;

    var aboutUs = urlService.GetAbsoluteUrl(nameof(Controllers.Content), nameof(Controllers.Content.AboutUs));
    yield return aboutUs;

    var dataPrivacyPolicy = urlService.GetAbsoluteUrl(nameof(Controllers.Content), nameof(Controllers.Content.DataPrivacyPolicy));
    yield return dataPrivacyPolicy;

    var imprint = urlService.GetAbsoluteUrl(nameof(Controllers.Content), nameof(Controllers.Content.Imprint));
    yield return imprint;

    var reportIssue = urlService.GetAbsoluteUrl(nameof(Controllers.Content), nameof(Controllers.Content.ReportIssue));
    yield return reportIssue;

    var feeds = urlService.GetAbsoluteUrl(nameof(Feeds), nameof(Index));
    yield return feeds;

    var login = urlService.GetAbsoluteUrl(nameof(Account), nameof(Account.Login));
    yield return login;

    var createEvent = urlService.GetAbsoluteUrl(nameof(Event), nameof(Event.Create));
    yield return createEvent;
  }
}
