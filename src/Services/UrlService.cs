using System.Web;
using GruenesBrett.Controllers;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;

namespace GruenesBrett.Services;

public class UrlService(IHttpContextAccessor accessor, LinkGenerator generator) : IUrlService
{
  /// <inheritdoc />
  public string? GetPermalink(SingleEvent singleEvent)
  {
    if (accessor?.HttpContext is null)
      return string.Empty;

    var eventId = singleEvent.ExternalId;
    var values = new { eventId };
    return GetAbsoluteUrl(nameof(Event), nameof(Event.Index), values);
  }

  /// <inheritdoc />
  public string? GetEncodedPermalink(SingleEvent singleEvent)
  {
    var permalink = GetPermalink(singleEvent);
    return HttpUtility.UrlEncode(permalink);
  }

  /// <inheritdoc />
  public string GetAbsoluteUrl(string controller, string action, object? values = null)
  {
    var httpContext = accessor.HttpContext;
    if (httpContext?.Request is null)
      return string.Empty;

    var host = httpContext.Request.Host;
    var protocol = httpContext.Request.Scheme;
    return generator.GetUriByAction(httpContext, action, controller, values, protocol, host) ?? string.Empty;
  }
}
