using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using Microsoft.EntityFrameworkCore;

namespace GruenesBrett.Services;

public class AuditService(ApplicationDbContext context, ILogger<AuditService> logger) : IAuditService
{
  /// <inheritdoc />
  public async Task LogAccountActivityAsync(string? message, params object?[] args)
  {
    var audit = new Audit
    {
      Id = Guid.CreateVersion7(),
      Message = message is not null ? message.FormatWith(args) : string.Empty,
      Timestamp = DateTime.UtcNow,
      Section = Constants.Audit.Account
    };

    context.Audits.Add(audit);
    await context.SaveChangesAsync();

    logger.LogInformation(message, args);
  }

  /// <inheritdoc />
  public async Task LogEventActivityAsync(string? message, params object?[] args)
  {
    var audit = new Audit
    {
      Id = Guid.CreateVersion7(),
      Message = message is not null ? message.FormatWith(args) : string.Empty,
      Timestamp = DateTime.UtcNow,
      Section = Constants.Audit.Event
    };

    context.Audits.Add(audit);
    await context.SaveChangesAsync();

    logger.LogInformation(message, args);
  }

  /// <inheritdoc />
  public async Task<List<Audit>> GetNewestActivities(string section)
  {
    var auditEntries = context.Audits.Where(a => a.Section == section);
    auditEntries = auditEntries.OrderByDescending(a => a.Timestamp);
    auditEntries = auditEntries.Take(100);
    return await auditEntries.ToListAsync();
  }
}
