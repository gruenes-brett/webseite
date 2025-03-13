using MailKit.Security;

namespace GruenesBrett.Email;

/// <summary>
/// The configuration for sending emails (using the MailKit library)
/// </summary>
public class MailKitEmailSenderOptions
{
  /// <summary>
  /// Constructor with default values
  /// </summary>
  public MailKitEmailSenderOptions()
  {
    Address = "localhost";
    Port = 587;
    Username = string.Empty;
    Password = string.Empty;
    SecureSocketOptions = SecureSocketOptions.Auto;
    SenderEmail = string.Empty;
    SenderName = string.Empty;
  }

  /// <summary>
  /// The address of the email server
  /// </summary>
  public string Address { get; set; }

  /// <summary>
  /// The port of the email server
  /// </summary>
  public int Port { get; set; }

  /// <summary>
  /// The (optional) username for authenticating on the email server
  /// </summary>
  public string Username { get; set; }

  /// <summary>
  /// The (optional) password for authenticating on the email server
  /// </summary>
  public string Password { get; set; }

  /// <summary>
  /// The type of encryption used for communicating with the email server
  /// </summary>
  public SecureSocketOptions SecureSocketOptions { get; set; }

  /// <summary>
  /// The email address that should be used as the sender
  /// </summary>
  public string SenderEmail { get; set; }

  /// <summary>
  /// The name that should be used as the sender
  /// </summary>
  public string SenderName { get; set; }
}
