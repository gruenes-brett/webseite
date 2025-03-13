using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GruenesBrett.Services;

public class EmailService(UserManager<ApplicationUser> userManager, ISettingsService settingsService,
  ILogger<EmailService> logger, IEmailSender emailSender) : IEmailService
{
  /// <inheritdoc />
  public async Task SendEmailAsync(string recipient, string subject, string body, EmailType type = EmailType.Optional)
  {
    var shouldSendEmail = await ShouldSendEmailAsync(recipient, type);
    if (shouldSendEmail)
      await emailSender.SendEmailAsync(recipient, subject, body);
  }

  /// <summary>
  /// Returns whether emails of the given type should be sent to the given recipient or not
  /// </summary>
  /// <param name="recipient"></param>
  /// <param name="type"></param>
  /// <returns></returns>
  private async Task<bool> ShouldSendEmailAsync(string recipient, EmailType type)
  {
    var isInvalidRecipient = recipient.IsNullOrWhiteSpace();
    if (isInvalidRecipient)
    {
      logger.LogInformation("Not sending email to {recipient} because they are invalid", recipient);
      return false;
    }

    var globalOptOutSetting = await settingsService.GetSetSettingAsync(Constants.Settings.GlobalOptOut);
    if (globalOptOutSetting.Contains(recipient))
    {
      logger.LogInformation("Not sending email to {recipient} because they are in the global opt-out setting", recipient);
      return false;
    }

    if (type == EmailType.System)
      return true;

    var user = await userManager.FindByEmailAsync(recipient);
    if (user is null)
      return true;

    if (user.Banned)
    {
      logger.LogInformation("Not sending email to {recipient} because they are banned", recipient);
      return false;
    }

    if (!user.ReceiveOptionalEmails && type == EmailType.Optional)
    {
      logger.LogInformation("Not sending email to {recipient} because they do not want to receive optional emails", recipient);
      return false;
    }

    if (!user.ReceiveReportingEmails && type == EmailType.Reporting)
    {
      logger.LogInformation("Not sending email to {recipient} because they do not want to receive reporting emails", recipient);
      return false;
    }

    return true;
  }
}
