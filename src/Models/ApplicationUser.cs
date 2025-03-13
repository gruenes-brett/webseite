using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Models;

public class ApplicationUser : IdentityUser
{
  public required string DisplayName { get; set; }

  public bool Banned { get; set; }

  public required DateTime Created { get; set; }

  public required DateTime PasswordChanged { get; set; }

  public DateTime? LastLogin { get; set; }

  public bool ReceiveOptionalEmails { get; set; }

  public bool ReceiveReportingEmails { get; set; }

  public string? DefaultPage { get; set; }

  [Column(TypeName = "geography")]
  public required Point Coordinates { get; set; }

  public required int Radius { get; set; }
}
