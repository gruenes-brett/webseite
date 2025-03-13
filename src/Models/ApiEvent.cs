namespace GruenesBrett.Models;

/// <summary>
/// An event in the REST API
/// </summary>
/// <param name="createFrom"></param>
public class ApiEvent(SingleEvent createFrom)
{
  public string Id { get; set; } = createFrom.ExternalId;

  public string? EventName { get; set; } = createFrom.EventName;

  public string? OrganizerName { get; set; } = createFrom.OrganizerName;

  public string? EventLink { get; set; } = createFrom.EventLink;

  public DateOnly? StartDate { get; set; } = createFrom.StartDate;

  public DateOnly? EndDate { get; set; } = createFrom.EndDate;

  public TimeOnly? StartTime { get; set; } = createFrom.StartTime;

  public TimeOnly? EndTime { get; set; } = createFrom.EndTime;

  public bool JoinEveryDay { get; set; } = createFrom.JoinEveryDay;

  public string? LocationName { get; set; } = createFrom.EventLocation?.Name;

  public string? Address { get; set; } = createFrom.EventLocation?.Address;

  public double? Latitude { get; set; } = createFrom.EventLocation?.Coordinates?.X;

  public double? Longitude { get; set; } = createFrom.EventLocation?.Coordinates?.Y;

  public string? EventDescription { get; set; } = createFrom.EventDescription;

  public string? PrimaryCategory { get; set; } = createFrom.PrimaryCategory?.Name;

  public IEnumerable<string> AdditionalCategories { get; set; } = createFrom.AdditionalCategories?.Select(c => c.Name) ?? [];

  public string? ImageFileName { get; set; } = createFrom.EventImage?.FileName;

  public int? ImageWidth { get; set; } = createFrom.EventImage?.Width;

  public int? ImageHeight { get; set; } = createFrom.EventImage?.Height;

  public bool IsReoccurring { get; set; } = createFrom.IsReoccurring;
}
