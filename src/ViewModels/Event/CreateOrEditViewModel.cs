using System.ComponentModel.DataAnnotations;
using GruenesBrett.Extensions;
using GruenesBrett.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GruenesBrett.ViewModels.Event;

public class CreateOrEditViewModel
{
  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "EventName", ResourceType = typeof(SystemTexts))]
  public required string EventName { get; init; }

  [Display(Name = "OrganizerName", ResourceType = typeof(SystemTexts))]
  public string? OrganizerName { get; init; }

  [DataType(DataType.Url)]
  [Display(Name = "EventLink", ResourceType = typeof(SystemTexts))]
  public string? EventLink { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [DataType(DataType.Date)]
  [Display(Name = "StartDate", ResourceType = typeof(SystemTexts))]
  public required DateOnly StartDate { get; init; }

  [DataType(DataType.Time)]
  [Display(Name = "StartTime", ResourceType = typeof(SystemTexts))]
  public TimeOnly? StartTime { get; init; }

  [DataType(DataType.Date)]
  [Display(Name = "EndDate", ResourceType = typeof(SystemTexts))]
  public DateOnly? EndDate { get; init; }

  [DataType(DataType.Time)]
  [Display(Name = "EndTime", ResourceType = typeof(SystemTexts))]
  public TimeOnly? EndTime { get; init; }

  [Display(Name = "JoinEveryDay", ResourceType = typeof(SystemTexts))]
  public bool JoinEveryDay { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "LocationName", ResourceType = typeof(SystemTexts))]
  public required string LocationName { get; init; }

  [Display(Name = "LocationAddress", ResourceType = typeof(SystemTexts))]
  public string? LocationAddress { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Longitude", ResourceType = typeof(SystemTexts))]
  public required string Longitude { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "Latitude", ResourceType = typeof(SystemTexts))]
  public required string Latitude { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [StringLength(2000, MinimumLength = 20, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "EventDescription", ResourceType = typeof(SystemTexts))]
  public required string EventDescription { get; init; }

  [Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "PrimaryCategory", ResourceType = typeof(SystemTexts))]
  public required string? PrimaryCategory { get; init; }

  [Display(Name = "AdditionalCategories", ResourceType = typeof(SystemTexts))]
  public List<string>? AdditionalCategories { get; init; }

  [Display(Name = "EventImage", ResourceType = typeof(SystemTexts))]
  public IFormFile? EventImage { get; init; }

  [Display(Name = "ConfirmImageRights", ResourceType = typeof(SystemTexts))]
  public bool ConfirmImageRights { get; init; }

  [MustBeChecked(ErrorMessageResourceName = "MustBeChecked", ErrorMessageResourceType = typeof(SystemTexts))]
  [Display(Name = "ConfirmPrivacyPolicy", ResourceType = typeof(SystemTexts))]
  public required bool ConfirmPrivacyPolicy { get; init; }

  public required IEnumerable<SelectListItem> AvailableCategories { get; set; }

  public int EventImageCropLeft { get; init; }

  public int EventImageCropTop { get; init; }

  public int EventImageCropWidth { get; init; }
}
