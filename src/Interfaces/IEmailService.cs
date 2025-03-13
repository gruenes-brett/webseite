namespace GruenesBrett.Interfaces;

/// <summary>
/// Handles sending emails
/// </summary>
public interface IEmailService
{
  /// <summary>
  /// Sends the email to the given recipient with the given subject and body
  /// </summary>
  /// <param name="recipient"></param>
  /// <param name="subject"></param>
  /// <param name="body"></param>
  /// <param name="type"></param>
  /// <returns></returns>
  Task SendEmailAsync(string recipient, string subject, string body, EmailType type = EmailType.Optional);
}

/// <summary>
/// The type of email
/// </summary>
public enum EmailType
{
  System,
  Optional,
  Reporting
}
