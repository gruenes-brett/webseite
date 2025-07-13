using System.Reflection;
using GruenesBrett.Extensions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace GruenesBrett.Email;

/// <summary>
/// Handles sending emails (using the MailKit library)
/// </summary>
/// <param name="options"></param>
/// <param name="logger"></param>
public class MailKitEmailSender(IOptions<MailKitEmailSenderOptions> options, ILogger<MailKitEmailSender> logger) : IEmailSender
{
  public MailKitEmailSenderOptions Options { get; set; } = options.Value;

  /// <summary>
  /// Sends the email to the given email address with the given subject and message
  /// </summary>
  /// <param name="email"></param>
  /// <param name="subject"></param>
  /// <param name="message"></param>
  /// <returns></returns>
  public async Task SendEmailAsync(string email, string subject, string message)
  {
    var template = await GetEmailTemplateAsync();
    var text = template.Replace("{subject}", subject).Replace("{body}", message);
    var body = new TextPart(TextFormat.Html) { Text = text };

    var mimeMessage = new MimeMessage();
    mimeMessage.To.Add(MailboxAddress.Parse(email));
    mimeMessage.Sender = MailboxAddress.Parse(Options.SenderEmail);
    mimeMessage.Sender.Name = Options.SenderName;
    mimeMessage.From.Add(mimeMessage.Sender);
    mimeMessage.Subject = subject;
    mimeMessage.Body = body;

    using var smtp = new SmtpClient();
    await smtp.ConnectAsync(Options.Address, Options.Port, Options.SecureSocketOptions);
    if (Options.Username.HasValue() && Options.Password.HasValue())
      await smtp.AuthenticateAsync(Options.Username, Options.Password);
    await smtp.SendAsync(mimeMessage);
    await smtp.DisconnectAsync(true);
  }

  /// <summary>
  /// Returns the content of the email template file, containing the markup
  /// </summary>
  /// <returns></returns>
  private async Task<string> GetEmailTemplateAsync()
  {
    try
    {
      var assembly = Assembly.GetExecutingAssembly();
      const string resourceName = "GruenesBrett.Email.EmailTemplate.html";

      await using var stream = assembly.GetManifestResourceStream(resourceName);
      if (stream is null)
        return string.Empty;

      using var reader = new StreamReader(stream);
      if (reader is null)
        return string.Empty;

      return await reader.ReadToEndAsync();
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to get email template");
      throw new FileNotFoundException();
    }
  }
}
