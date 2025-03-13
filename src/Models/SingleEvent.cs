using System.ComponentModel.DataAnnotations;

namespace GruenesBrett.Models;

public class SingleEvent
{
  [Key]
  public required Guid InternalId { get; set; }

  public required string ExternalId { get; set; }

  public DateTime Created { get; set; }

  public DateTime Updated { get; set; }

  public ApplicationUser? CreatedBy { get; set; }

  public string? CreatedByName { get; set; }

  public string? CreatedByEmail { get; set; }

  public required string EventName { get; set; }

  public string? OrganizerName { get; set; }

  public string? EventLink { get; set; }

  public required DateOnly StartDate { get; set; }

  public DateOnly? EndDate { get; set; }

  public TimeOnly? StartTime { get; set; }

  public TimeOnly? EndTime { get; set; }

  public bool JoinEveryDay { get; set; }

  public required Location EventLocation { get; set; }

  public required string EventDescription { get; set; }

  public required Category PrimaryCategory { get; set; }

  public required List<Category> AdditionalCategories { get; set; }

  public Image? EventImage { get; set; }

  public bool IsReoccurring { get; set; }

  public DateOnly RelevantDate => EndDate ?? StartDate;

  public DateTime? LastReported { get; set; }

  public WorkflowStatus WorkflowStatus { get; set; }
}
