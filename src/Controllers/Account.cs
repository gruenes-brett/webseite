using System.Text;
using System.Text.Encodings.Web;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles all self-service actions for a user
/// (like logging in, resetting the password or editing user name)
/// </summary>
/// <param name="auditService"></param>
/// <param name="emailService"></param>
/// <param name="logger"></param>
/// <param name="signInManager"></param>
/// <param name="textService"></param>
/// <param name="urlService"></param>
/// <param name="userManager"></param>
/// <param name="userService"></param>
[Route("account")]
public class Account(IAuditService auditService, IEmailService emailService, ILogger<Account> logger,
  ISettingsService settingsService, SignInManager<ApplicationUser> signInManager, ITextService textService,
  IUrlService urlService, UserManager<ApplicationUser> userManager, IUserService userService) : Controller
{
  /// <summary>
  /// Shows the self-registration form
  /// </summary>
  /// <returns></returns>
  [Route("registrieren")]
  public async Task<IActionResult> Register()
  {
    var isSignedIn = signInManager.IsSignedIn(User);
    if (isSignedIn)
      return RedirectToAction(nameof(Index));

    var selfRegistrationEnabled = await settingsService.GetBoolSettingAsync(Constants.Settings.SelfRegistration);
    var hasNoUsers = await userService.HasNoUsers();
    if (!selfRegistrationEnabled && !hasNoUsers)
      return RedirectToAction(nameof(Login));

    return View();
  }

  /// <summary>
  /// Processes the inputs of the self-registration form
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("registrieren")]
  public async Task<IActionResult> Register(RegistrationViewModel viewModel)
  {
    var selfRegistrationEnabled = await settingsService.GetBoolSettingAsync(Constants.Settings.SelfRegistration);
    var hasNoUsers = await userService.HasNoUsers();
    if (!selfRegistrationEnabled && !hasNoUsers)
      return RedirectToAction(nameof(Login));

    if (!ModelState.IsValid)
      return View();

    const double longitude = Constants.Locations.DefaultLongitude;
    const double latitude = Constants.Locations.DefaultLatitude;

    var user = new ApplicationUser
    {
      DisplayName = viewModel.Name,
      UserName = viewModel.Email,
      Email = viewModel.Email,
      Created = DateTime.UtcNow,
      PasswordChanged = DateTime.UtcNow,
      ReceiveOptionalEmails = true,
      ReceiveReportingEmails = true,
      DefaultPage = Constants.Pages.MyAccount,
      Coordinates = new Point(longitude, latitude) { SRID = Constants.Locations.Srid },
      Radius = Constants.Locations.DefaultRadius
    };

    var result = await userManager.CreateAsync(user, viewModel.Password);
    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

      return View();
    }

    await auditService.LogAccountActivityAsync("User {email} registered successfully", user.Email);
    await AddInitialRole(user);

    var confirmationLink = await GetConfirmationLinkAsync(user);
    if (confirmationLink.IsNullOrEmpty())
    {
      var registrationError = textService.GetText(Constants.Text.Account.RegistrationError);
      ModelState.AddModelError(string.Empty, registrationError);
      return View();
    }

    await SendConfirmAccountEmailAsync(user, confirmationLink);
    await SendNewUserEmailAsync(user);
    return RedirectToAction(nameof(RegisterConfirmation));
  }

  /// <summary>
  /// Shows the information that self-registration was successful
  /// </summary>
  /// <returns></returns>
  [Route("registrierung-erfolgreich")]
  public IActionResult RegisterConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Processes the confirmation of the email address
  /// (this page is linked in the email sent to the user)
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="code"></param>
  /// <returns></returns>
  [Route("email-bestaetigen")]
  public async Task<IActionResult> ConfirmEmail(string userId, string code)
  {
    if (userId.IsNullOrEmpty() || code.IsNullOrEmpty())
      return View(model: textService.GetText(Constants.Text.Account.ConfirmEmailError));

    var user = await userManager.FindByIdAsync(userId);
    if (user is null)
      return View(model: textService.GetText(Constants.Text.Account.ConfirmEmailError));

    var alreadyConfirmed = user.EmailConfirmed;
    if (alreadyConfirmed)
      return View(model: textService.GetText(Constants.Text.Account.ConfirmEmailAlreadyConfirmed));

    var binaryCode = WebEncoders.Base64UrlDecode(code);
    var decodedCode = Encoding.UTF8.GetString(binaryCode);

    var result = await userManager.ConfirmEmailAsync(user, decodedCode);
    if (result.Succeeded)
    {
      await auditService.LogAccountActivityAsync("User {email} confirmed their email address", user.Email);
      return View(model: textService.GetText(Constants.Text.Account.ConfirmEmailSuccess));
    }

    return View(model: textService.GetText(Constants.Text.Account.ConfirmEmailError));
  }

  /// <summary>
  /// Processes the request to send another email with the email address confirmation link
  /// </summary>
  /// <param name="userId"></param>
  /// <returns></returns>
  [Route("email-bestaetigung-senden")]
  public async Task<IActionResult> ConfirmEmailTrigger(string userId)
  {
    if (userId.IsNullOrEmpty())
      return View();

    var user = await userManager.FindByEmailAsync(userId);
    if (user is null)
      return View();

    var alreadyConfirmed = user.EmailConfirmed;
    if (alreadyConfirmed)
      return View();

    var confirmationLink = await GetConfirmationLinkAsync(user);
    if (confirmationLink.IsNullOrEmpty())
      return View();

    await SendConfirmAccountEmailAsync(user, confirmationLink);
    return View();
  }

  /// <summary>
  /// Shows the login form
  /// </summary>
  /// <returns></returns>
  [Route("anmelden")]
  public IActionResult Login()
  {
    var isSignedIn = signInManager.IsSignedIn(User);
    if (isSignedIn)
      return RedirectToAction(nameof(Index));

    return View();
  }

  /// <summary>
  /// Processes the inputs of the login form
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("anmelden")]
  public async Task<IActionResult> Login(LoginViewModel viewModel)
  {
    var isSignedIn = signInManager.IsSignedIn(User);
    if (isSignedIn)
      return RedirectToAction(nameof(Index));

    if (!ModelState.IsValid)
      return View();

    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
    {
      var userNotFoundError = textService.GetText(Constants.Text.Account.LoginFailed);
      ModelState.AddModelError(string.Empty, userNotFoundError);
      return View();
    }

    if (user.Banned)
    {
      var bannedError = textService.GetText(Constants.Text.Account.Banned);
      ModelState.AddModelError(string.Empty, bannedError);
      return View();
    }

    var result = await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, true);
    if (result.Succeeded)
    {
      user.LastLogin = DateTime.UtcNow;
      await userManager.UpdateAsync(user);
      await auditService.LogAccountActivityAsync("User {email} logged in", viewModel.Email);

      if (viewModel.ReturnUrl is null)
      {
        return user.DefaultPage switch
        {
          Constants.Pages.Explore => RedirectToAction(nameof(Events.Explore), nameof(Events)),
          Constants.Pages.Calendar => RedirectToAction(nameof(Events.Calendar), nameof(Events)),
          Constants.Pages.Map => RedirectToAction(nameof(Events.Map), nameof(Events)),
          Constants.Pages.MyAccount => RedirectToAction(nameof(Index)),
          Constants.Pages.CreateEvent => RedirectToAction(nameof(Event.Create), nameof(Event)),
          _ => RedirectToAction(nameof(Index)),
        };
      }

      return LocalRedirect(viewModel.ReturnUrl);
    }

    if (result.IsLockedOut)
    {
      await auditService.LogAccountActivityAsync("User {email} was locked out after too many failed attempts", viewModel.Email);
      var lockedOutError = textService.GetText(Constants.Text.Account.LockedOut);
      ModelState.AddModelError(string.Empty, lockedOutError);
      return View();
    }

    if (result.IsNotAllowed)
    {
      await auditService.LogAccountActivityAsync("User {email} tried to log in while not having the email confirmed", viewModel.Email);
      var notConfirmedError = textService.GetText(Constants.Text.Account.NotConfirmed);
      var emailConfirmTriggerLink = Url.Action(nameof(ConfirmEmailTrigger), new { userId = viewModel.Email });
      var formattedNotConfirmedError = string.Format(notConfirmedError, emailConfirmTriggerLink);
      ModelState.AddModelError(nameof(LoginViewModel.ConfirmEmailTrigger), formattedNotConfirmedError);
      return View();
    }

    var loginFailedError = textService.GetText(Constants.Text.Account.LoginFailed);
    ModelState.AddModelError(string.Empty, loginFailedError);
    return View();
  }

  /// <summary>
  /// Processes the request to log the user out
  /// </summary>
  /// <returns></returns>
  [Route("abmelden")]
  public async Task<IActionResult> Logout()
  {
    await signInManager.SignOutAsync();
    await auditService.LogAccountActivityAsync("User {email} logged out", User?.Identity?.Name);
    return RedirectToAction(nameof(Events.Explore), nameof(Events));
  }

  /// <summary>
  /// Shows the forgot password form
  /// (for requesting an email with a link to set a new password)
  /// </summary>
  /// <returns></returns>
  [Route("passwort-vergessen")]
  public IActionResult ForgotPassword()
  {
    return View();
  }

  /// <summary>
  /// Processes the inputs of the forgot password form
  /// (and send an email with the link to reset the password)
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("passwort-vergessen")]
  public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View();

    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null || !await userManager.IsEmailConfirmedAsync(user))
      return RedirectToAction(nameof(ForgotPasswordConfirmation));

    var passwordResetLink = await GetPasswordResetLinkAsync(user);
    if (passwordResetLink.IsNullOrEmpty())
      return RedirectToAction(nameof(ForgotPasswordConfirmation));

    await SendPasswordResetEmailAsync(user, passwordResetLink);
    await auditService.LogAccountActivityAsync("User {email} triggered a password reset email", user.Email);
    return RedirectToAction(nameof(ForgotPasswordConfirmation));
  }

  /// <summary>
  /// Shows the information that requesting an email for resetting the password was successful
  /// </summary>
  /// <returns></returns>
  [Route("passwort-vergessen-bestaetigung")]
  public IActionResult ForgotPasswordConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Shows the form where a new password can be chosen
  /// (this page is linked from the reset password email)
  /// </summary>
  /// <returns></returns>
  [Route("passwort-zuruecksetzen")]
  public IActionResult ResetPassword(string? code = default)
  {
    if (code is null)
      return View();

    var binaryToken = WebEncoders.Base64UrlDecode(code);
    var decodedToken = Encoding.UTF8.GetString(binaryToken);
    var viewModel = new ResetPasswordViewModel
    {
      Email = string.Empty,
      Password = string.Empty,
      ConfirmPassword = string.Empty,
      Token = decodedToken
    };
    return View(viewModel);
  }

  /// <summary>
  /// Processes the inputs to the reset password form
  /// (where the user chose a new password)
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("passwort-zuruecksetzen")]
  public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
  {
    if (!ModelState.IsValid)
    {
      var resetPasswordError = textService.GetText(Constants.Text.Account.ResetPasswordError);
      ModelState.AddModelError(string.Empty, resetPasswordError);
      return View();
    }

    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(ResetPasswordConfirmation));

    var result = await userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
    if (result.Succeeded)
    {
      user.PasswordChanged = DateTime.UtcNow;

      await userManager.UpdateAsync(user);
      await auditService.LogAccountActivityAsync("User {email} reset their passwort", user.Email);
      return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View();
  }

  /// <summary>
  /// Shows the information that setting a new password was successful
  /// </summary>
  /// <returns></returns>
  [Route("passwort-zuruecksetzen-bestaetigung")]
  public IActionResult ResetPasswordConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Shows the account information page (with a form for account settings)
  /// </summary>
  /// <returns></returns>
  [Authorize]
  [Route("")]
  public async Task<IActionResult> Index()
  {
    var user = await userManager.GetUserAsync(User);
    if (user is null)
    {
      var editError = textService.GetText(Constants.Text.Account.EditError);
      ModelState.AddModelError(string.Empty, editError);
      return View();
    }

    var viewModel = new AccountSettingsViewModel
    {
      ReceiveOptionalEmails = user.ReceiveOptionalEmails,
      ReceiveReportingEmails = user.ReceiveReportingEmails,
      DefaultPage = user.DefaultPage
    };
    return View(viewModel);
  }

  /// <summary>
  /// Processes the inputs for the account settings form
  /// </summary>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize]
  [Route("")]
  public async Task<IActionResult> Index(AccountSettingsViewModel viewModel)
  {
    if (!ModelState.IsValid)
      View();

    var user = await userManager.GetUserAsync(User);
    if (user is null)
    {
      var editError = textService.GetText(Constants.Text.Account.EditError);
      ModelState.AddModelError(string.Empty, editError);
      return View();
    }

    user.ReceiveOptionalEmails = viewModel.ReceiveOptionalEmails;
    user.ReceiveReportingEmails = viewModel.ReceiveReportingEmails;
    user.DefaultPage = viewModel.DefaultPage;

    var result = await userManager.UpdateAsync(user);
    if (result.Succeeded)
      return View(viewModel);

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for editing the account display name
  /// </summary>
  /// <returns></returns>
  [Authorize]
  [Route("name-bearbeiten")]
  public async Task<IActionResult> EditName()
  {
    var user = await userManager.GetUserAsync(User);
    var viewModel = new EditNameViewModel
    {
      Name = user?.DisplayName ?? string.Empty
    };
    return View(viewModel);
  }

  /// <summary>
  /// Processes the inputs for the form for setting the account display name
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize]
  [Route("name-bearbeiten")]
  public async Task<IActionResult> EditName(EditNameViewModel viewModel)
  {
    if (!ModelState.IsValid)
      View();

    var user = await userManager.GetUserAsync(User);
    if (user is null)
    {
      var editError = textService.GetText(Constants.Text.Account.EditError);
      ModelState.AddModelError(string.Empty, editError);
      return View();
    }

    user.DisplayName = viewModel.Name;

    var result = await userManager.UpdateAsync(user);
    if (result.Succeeded)
      return RedirectToAction(nameof(Index));

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View();
  }

  /// <summary>
  /// Shows the form for editing the password
  /// </summary>
  /// <returns></returns>
  [Authorize]
  [Route("passwort-bearbeiten")]
  public IActionResult EditPassword()
  {
    return View();
  }

  /// <summary>
  /// Processes the inputs for the edit password form
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize]
  [Route("passwort-bearbeiten")]
  public async Task<IActionResult> EditPassword(EditPasswordViewModel viewModel)
  {
    if (!ModelState.IsValid)
      View();

    var user = await userManager.GetUserAsync(User);
    if (user is null)
    {
      var editError = textService.GetText(Constants.Text.Account.EditError);
      ModelState.AddModelError(string.Empty, editError);
      return View();
    }

    var token = await userManager.GeneratePasswordResetTokenAsync(user);
    var result = await userManager.ResetPasswordAsync(user, token, viewModel.Password);
    if (result.Succeeded)
    {
      user.PasswordChanged = DateTime.UtcNow;

      await userManager.UpdateAsync(user);
      await auditService.LogAccountActivityAsync("User {email} changed their passwort", user.Email);
      return RedirectToAction(nameof(EditPasswordConfirmation));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View();
  }

  /// <summary>
  /// Shows the information that editing the password was successful
  /// </summary>
  /// <returns></returns>
  [Authorize]
  [Route("passwort-bearbeiten-erfolgreich")]
  public IActionResult EditPasswordConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Returns the link for confirming the email address of the given user
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task<string> GetConfirmationLinkAsync(ApplicationUser user)
  {
    try
    {
      var userId = await userManager.GetUserIdAsync(user);
      var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
      var binaryToken = Encoding.UTF8.GetBytes(token);
      var code = WebEncoders.Base64UrlEncode(binaryToken);
      var url = urlService.GetAbsoluteUrl(nameof(Account), nameof(ConfirmEmail), new { userId, code });
      return HtmlEncoder.Default.Encode(url);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to generate confirmation link for user {email}", user.Email);
      return string.Empty;
    }
  }

  /// <summary>
  /// Returns the link for resetting the password of the given user
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task<string> GetPasswordResetLinkAsync(ApplicationUser user)
  {
    try
    {
      var token = await userManager.GeneratePasswordResetTokenAsync(user);
      var binaryToken = Encoding.UTF8.GetBytes(token);
      var code = WebEncoders.Base64UrlEncode(binaryToken);
      var url = urlService.GetAbsoluteUrl(nameof(Account), nameof(ResetPassword), new { code });
      return HtmlEncoder.Default.Encode(url);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to generate password reset token for user {email}", user.Email);
      return string.Empty;
    }
  }

  /// <summary>
  /// Sends the email to the given user with the given link for confirming their email address
  /// </summary>
  /// <param name="user"></param>
  /// <param name="confirmationLink"></param>
  /// <returns></returns>
  private async Task SendConfirmAccountEmailAsync(ApplicationUser user, string confirmationLink)
  {
    try
    {
      var subject = textService.GetText(Constants.Text.Account.ConfirmAccountEmailSubject);
      var body = textService.GetText(Constants.Text.Account.ConfirmAccountEmailBody);
      body = string.Format(body, confirmationLink);
      if (user.Email is not null)
        await emailService.SendEmailAsync(user.Email, subject, body, EmailType.System);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send confirm account email for user {email}", user.Email);
    }
  }

  /// <summary>
  /// Sends the email for approving the given user to all administrators
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task SendNewUserEmailAsync(ApplicationUser user)
  {
    try
    {
      var isFirstUser = await userService.IsFirstUser();
      if (isFirstUser)
        return;

      var approvalLink = urlService.GetAbsoluteUrl(nameof(Accounts), nameof(Accounts.Approve), new { email = user.Email });
      var subject = textService.GetText(Constants.Text.Account.NewUserEmailSubject);
      var body = textService.GetText(Constants.Text.Account.NewUserEmailBody);
      body = string.Format(body, approvalLink);

      var administrators = await userManager.GetUsersInRoleAsync(Constants.Roles.Administrator);
      if (administrators is null)
        return;

      foreach (var administrator in administrators)
        await emailService.SendEmailAsync(administrator.Email!, subject, body, EmailType.Optional);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send new user email to user {email}", user.Email);
    }
  }

  /// <summary>
  /// Sends the email to the given user with the given link for resetting their password
  /// </summary>
  /// <param name="user"></param>
  /// <param name="passwordResetLink"></param>
  /// <returns></returns>
  private async Task SendPasswordResetEmailAsync(ApplicationUser user, string passwordResetLink)
  {
    try
    {
      var subject = textService.GetText(Constants.Text.Account.ResetPasswordEmailSubject);
      var body = textService.GetText(Constants.Text.Account.ResetPasswordEmailBody);
      body = string.Format(body, passwordResetLink);
      await emailService.SendEmailAsync(user.Email!, subject, body, EmailType.System);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send password reset email for user {email}", user.Email);
    }
  }

  /// <summary>
  /// Adds the administrator role to the given user if it is the first account on the site
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task AddInitialRole(ApplicationUser user)
  {
    var isFirstUser = await userService.IsFirstUser();
    if (!isFirstUser)
      return;

    const string administrator = Constants.Roles.Administrator;
    var result = await userManager.AddToRoleAsync(user, administrator);
    if (result.Succeeded)
      logger.LogInformation("Added role {administrator} to first user {email}", administrator, user.Email);
  }
}
