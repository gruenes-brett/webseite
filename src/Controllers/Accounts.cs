using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using GruenesBrett.Extensions;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GruenesBrett.Controllers;

/// <summary>
/// Handles managing other accounts
/// </summary>
/// <param name="auditService"></param>
/// <param name="emailService"></param>
/// <param name="logger"></param>
/// <param name="textService"></param>
/// <param name="urlService"></param>
/// <param name="userManager"></param>
[Authorize(Roles = Constants.Roles.Administrator)]
[Route("accounts")]
public class Accounts(IAuditService auditService, IEmailService emailService, ILogger<Account> logger,
  ITextService textService, IUrlService urlService, UserManager<ApplicationUser> userManager) : Controller
{
  /// <summary>
  /// Shows the form for creating a new account
  /// </summary>
  /// <returns></returns>
  [Route("anlegen")]
  public IActionResult Create()
  {
    var viewModel = new CreateAccountViewModel
    {
      Name = string.Empty,
      Email = string.Empty,
      Role = Constants.Roles.Normal
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles inputs for the form for creating a new account
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("anlegen")]
  public async Task<IActionResult> Create(CreateAccountViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View(viewModel);

    const double longitude = Constants.Locations.DefaultLongitude;
    const double latitude = Constants.Locations.DefaultLatitude;

    var user = new ApplicationUser
    {
      DisplayName = viewModel.Name,
      UserName = viewModel.Email,
      Email = viewModel.Email,
      Created = DateTime.UtcNow,
      PasswordChanged = DateTime.MinValue,
      ReceiveOptionalEmails = true,
      ReceiveReportingEmails = true,
      DefaultPage = Constants.Pages.MyAccount,
      Coordinates = new Point(longitude, latitude) { SRID = Constants.Locations.Srid },
      Radius = Constants.Locations.DefaultRadius
    };

    var createResult = await userManager.CreateAsync(user);
    if (!createResult.Succeeded)
    {
      foreach (var error in createResult.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

      return View(viewModel);
    }

    await userManager.AddToRoleAsync(user, viewModel.Role);
    var email = user.Email;
    var admin = User?.Identity?.Name;
    var role = viewModel.Role;
    await auditService.LogAccountActivityAsync("User {email} was created by user {admin} and assigned role {role}", email, admin, role);

    var confirmationLink = await GetConfirmationLinkAsync(user);
    if (confirmationLink.IsNullOrEmpty())
    {
      var registrationError = textService.GetText(Constants.Text.Accounts.CreateError);
      ModelState.AddModelError(string.Empty, registrationError);
      return View(viewModel);
    }

    await SendSetPasswordEmailAsync(user, confirmationLink);
    return RedirectToAction(nameof(Index));
  }

  /// <summary>
  /// Shows the form for setting the initial password for the user
  /// with the given userId and the given authorization code
  /// </summary>
  /// <returns></returns>
  [AllowAnonymous]
  [Route("passwort-vergeben")]
  public async Task<IActionResult> SetPassword(string userId, string code)
  {
    var viewModel = new SetPasswordViewModel
    {
      UserId = userId,
      Code = code,
      Password = string.Empty,
      ConfirmPassword = string.Empty
    };

    var parametersAreInvalid = userId.IsNullOrEmpty() || code.IsNullOrEmpty();
    if (parametersAreInvalid)
    {
      var confirmEmailError = textService.GetText(Constants.Text.Account.ConfirmEmailError);
      ModelState.AddModelError(string.Empty, confirmEmailError);
      return View(viewModel);
    }

    var user = await userManager.FindByIdAsync(userId);
    if (user is null)
    {
      var confirmEmailError = textService.GetText(Constants.Text.Account.ConfirmEmailError);
      ModelState.AddModelError(string.Empty, confirmEmailError);
      return View(viewModel);
    }

    var alreadyHasPassword = await userManager.HasPasswordAsync(user);
    if (alreadyHasPassword)
    {
      var alreadyHasPasswordError = textService.GetText(Constants.Text.Accounts.AlreadyHasPassword);
      ModelState.AddModelError(string.Empty, alreadyHasPasswordError);
      return View(viewModel);
    }

    var alreadyConfirmed = user.EmailConfirmed;
    if (alreadyConfirmed)
    {
      var alreadyConfirmedError = textService.GetText(Constants.Text.Account.ConfirmEmailAlreadyConfirmed);
      ModelState.AddModelError(string.Empty, alreadyConfirmedError);
      return View(viewModel);
    }

    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for setting the initial password for the user
  /// </summary>
  /// <returns></returns>
  [AllowAnonymous]
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("passwort-vergeben")]
  public async Task<IActionResult> SetPassword(SetPasswordViewModel viewModel)
  {
    if (!ModelState.IsValid)
      return View(viewModel);

    var user = await userManager.FindByIdAsync(viewModel.UserId);
    if (user is null)
    {
      var confirmEmailError = textService.GetText(Constants.Text.Account.ConfirmEmailError);
      ModelState.AddModelError(string.Empty, confirmEmailError);
      return View(viewModel);
    }

    var alreadyHasPassword = await userManager.HasPasswordAsync(user);
    if (alreadyHasPassword)
    {
      var alreadyHasPasswordError = textService.GetText(Constants.Text.Accounts.AlreadyHasPassword);
      ModelState.AddModelError(string.Empty, alreadyHasPasswordError);
      return View(viewModel);
    }

    var alreadyConfirmed = user.EmailConfirmed;
    if (alreadyConfirmed)
    {
      var alreadyConfirmedError = textService.GetText(Constants.Text.Account.ConfirmEmailAlreadyConfirmed);
      ModelState.AddModelError(string.Empty, alreadyConfirmedError);
      return View(viewModel);
    }

    var binaryCode = WebEncoders.Base64UrlDecode(viewModel.Code);
    var decodedCode = Encoding.UTF8.GetString(binaryCode);

    var confirmEmailResult = await userManager.ConfirmEmailAsync(user, decodedCode);
    if (!confirmEmailResult.Succeeded)
    {
      foreach (var error in confirmEmailResult.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

      return View(viewModel);
    }

    var addPasswordResult = await userManager.AddPasswordAsync(user, viewModel.Password);
    if (!addPasswordResult.Succeeded)
    {
      foreach (var error in addPasswordResult.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

      await auditService.LogAccountActivityAsync("User {email} confirmed their email address, but did not set a password", user.Email);
      return View(viewModel);
    }

    user.PasswordChanged = DateTime.UtcNow;
    await userManager.UpdateAsync(user);
    await auditService.LogAccountActivityAsync("User {email} confirmed their email address and set a password", user.Email);
    return RedirectToAction(nameof(SetPasswordConfirmation));
  }

  /// <summary>
  /// Shows the information that setting the initial password was successful
  /// </summary>
  /// <returns></returns>
  [AllowAnonymous]
  [Route("passwort-vergeben-erfolgreich")]
  public IActionResult SetPasswordConfirmation()
  {
    return View();
  }

  /// <summary>
  /// Shows the list of all accounts
  /// </summary>
  /// <returns></returns>
  [Route("")]
  public async Task<IActionResult> Index()
  {
    var users = await userManager.Users.OrderBy(u => u.DisplayName).ToListAsync() ?? [];
    var accounts = users.Select(async user => await MapUserToAccount(user)).Select(t => t.Result).OfType<AccountViewModel>() ?? [];
    var viewModel = new EditAccountsViewModel
    {
      Accounts = accounts
    };
    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for approving the user with the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  [Route("freigeben")]
  public async Task<IActionResult> Approve(string email)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    var viewModel = CreateAccountActionViewModel(user);
    var roles = await userManager.GetRolesAsync(user);
    var alreadyApproved = roles?.Count > 0;
    if (alreadyApproved)
    {
      var alreadyApprovedError = textService.GetText(Constants.Text.Accounts.AlreadyApproved);
      ModelState.AddModelError(string.Empty, alreadyApprovedError);
    }

    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for approving the user with the given email address
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("freigeben")]
  public async Task<IActionResult> Approve(AccountActionViewModel viewModel)
  {
    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    var roles = await userManager.GetRolesAsync(user);
    var alreadyApproved = roles?.Count > 0;
    if (alreadyApproved)
    {
      var alreadyApprovedError = textService.GetText(Constants.Text.Accounts.AlreadyApproved);
      ModelState.AddModelError(string.Empty, alreadyApprovedError);
      return View(viewModel);
    }

    var result = await userManager.AddToRoleAsync(user, Constants.Roles.Normal);
    if (result.Succeeded)
    {
      var email = user.Email;
      var admin = User?.Identity?.Name;

      await auditService.LogAccountActivityAsync("User {email} was approved by user {admin}", email, admin);
      await SendApprovalEmailAsync(user);
      return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Show the form for banning the user with the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  [Route("sperren")]
  public async Task<IActionResult> Ban(string email)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    var viewModel = CreateAccountActionViewModel(user);
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for banning the user with the given email address
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("sperren")]
  public async Task<IActionResult> Ban(AccountActionViewModel viewModel)
  {
    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    if (user.Email == User?.Identity?.Name)
    {
      var banError = textService.GetText(Constants.Text.Accounts.BanOrUnbanYourself);
      ModelState.AddModelError(string.Empty, banError);
      return View(viewModel);
    }

    user.Banned = true;

    var result = await userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
      var email = user.Email;
      var admin = User?.Identity?.Name;
      await auditService.LogAccountActivityAsync("User {email} was banned by user {admin}", email, admin);
      return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for unbanning the user with the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  [Route("entsperren")]
  public async Task<IActionResult> Unban(string email)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
      var unbanError = textService.GetText(Constants.Text.Account.EditError);
      ModelState.AddModelError(string.Empty, unbanError);
      return View();
    }

    var viewModel = CreateAccountActionViewModel(user);
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for unbanning the user with the given email address
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("entsperren")]
  public async Task<IActionResult> Unban(AccountActionViewModel viewModel)
  {
    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    if (user.Email == User?.Identity?.Name)
    {
      var unbanError = textService.GetText(Constants.Text.Accounts.BanOrUnbanYourself);
      ModelState.AddModelError(string.Empty, unbanError);
      return View(viewModel);
    }

    user.Banned = false;

    var result = await userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
      var email = user.Email;
      var admin = User?.Identity?.Name;
      await auditService.LogAccountActivityAsync("User {email} was unbanned by user {admin}", email, admin);
      return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for deleting the user with the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  [Route("loeschen")]
  public async Task<IActionResult> Delete(string email)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    var viewModel = CreateAccountActionViewModel(user);
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for deleting the user with the given email address
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("loeschen")]
  public async Task<IActionResult> Delete(AccountActionViewModel viewModel)
  {
    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    if (user.Email == User?.Identity?.Name)
    {
      var deleteError = textService.GetText(Constants.Text.Accounts.DeleteYourself);
      ModelState.AddModelError(string.Empty, deleteError);
      return View(viewModel);
    }

    var result = await userManager.DeleteAsync(user);
    if (result.Succeeded)
    {
      var email = user.Email;
      var admin = User?.Identity?.Name;

      await auditService.LogAccountActivityAsync("User {email} was deleted by user {admin}", email, admin);
      return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for editing the role of the user with the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  [Route("rolle-bearbeiten")]
  public async Task<IActionResult> EditRole(string email)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    var roles = await userManager.GetRolesAsync(user);
    var firstRole = roles.FirstOrDefault();
    var viewModel = new EditRoleViewModel
    {
      Name = user.DisplayName ?? string.Empty,
      Email = user.Email ?? string.Empty,
      Created = DateOnly.FromDateTime(user.Created),
      Role = firstRole ?? string.Empty
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for editing the role of the user with the given email address
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("rolle-bearbeiten")]
  public async Task<IActionResult> EditRole(EditRoleViewModel viewModel)
  {
    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    if (user.Email == User?.Identity?.Name)
    {
      var editRoleError = textService.GetText(Constants.Text.Accounts.EditRoleOfYourself);
      ModelState.AddModelError(string.Empty, editRoleError);
      return View(viewModel);
    }

    var rolesToRemove = await userManager.GetRolesAsync(user);
    var removedRoles = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
    if (!removedRoles.Succeeded)
    {
      foreach (var error in removedRoles.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

      return View(viewModel);
    }

    var addedRole = await userManager.AddToRoleAsync(user, viewModel.Role);
    if (!addedRole.Succeeded)
    {
      foreach (var error in addedRole.Errors)
        ModelState.AddModelError(string.Empty, error.Description);

      return View(viewModel);
    }

    var result = await userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
      var email = user.Email;
      var role = viewModel.Role;
      var admin = User?.Identity?.Name;

      await auditService.LogAccountActivityAsync("User {email} got role {role} from user {admin}", email, role, admin);
      return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Shows the form for editing the responsibility area of the user with the given email address
  /// </summary>
  /// <param name="email"></param>
  /// <returns></returns>
  [Route("zustaendigkeit-bearbeiten")]
  public async Task<IActionResult> EditResponsibility(string email)
  {
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    var roles = await userManager.GetRolesAsync(user);
    var firstRole = roles.FirstOrDefault();

    var longitude = user.Coordinates.X.ToString(NumberFormatInfo.InvariantInfo);
    var latitude = user.Coordinates.Y.ToString(NumberFormatInfo.InvariantInfo);

    var viewModel = new EditResponsibilityViewModel
    {
      Name = user.DisplayName ?? string.Empty,
      Email = user.Email ?? string.Empty,
      Role = firstRole ?? string.Empty,
      Radius = user.Radius,
      Latitude = latitude,
      Longitude = longitude
    };
    return View(viewModel);
  }

  /// <summary>
  /// Handles the inputs for the form for editing the responsibility
  /// area of the user with the given email address
  /// </summary>
  /// <param name="viewModel"></param>
  /// <returns></returns>
  [HttpPost]
  [ValidateAntiForgeryToken]
  [Route("zustaendigkeit-bearbeiten")]
  public async Task<IActionResult> EditResponsibility(EditResponsibilityViewModel viewModel)
  {
    var user = await userManager.FindByEmailAsync(viewModel.Email);
    if (user is null)
      return RedirectToAction(nameof(Index));

    if (viewModel.Radius.HasValue)
    {
      const int min = Constants.Locations.MinRadius;
      const int max = Constants.Locations.MaxRadius;
      user.Radius = int.Clamp(viewModel.Radius.Value, min, max);
    }

    if (viewModel.Latitude.HasValue() && viewModel.Longitude.HasValue())
    {
      var isValidLongitude = double.TryParse(viewModel.Longitude, NumberFormatInfo.InvariantInfo, out var longitude);
      var isValidLatitude = double.TryParse(viewModel.Latitude, NumberFormatInfo.InvariantInfo, out var latitude);

      if (isValidLongitude && isValidLatitude)
        user.Coordinates = new Point(longitude, latitude) { SRID = Constants.Locations.Srid };
    }

    var result = await userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
      var email = user.Email;
      var admin = User?.Identity?.Name;

      await auditService.LogAccountActivityAsync("User {email} got different responsibility area from user {admin}", email, admin);
      return RedirectToAction(nameof(Index));
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    return View(viewModel);
  }

  /// <summary>
  /// Shows the log with recent account activities
  /// </summary>
  /// <returns></returns>
  [Route("aktivitaeten-ansehen")]
  public async Task<IActionResult> ShowAuditLog()
  {
    var auditEntries = await auditService.GetNewestActivities(Constants.Audit.Account);
    return View(auditEntries);
  }

  /// <summary>
  /// Maps the given user to a view model for showing
  /// it in the list of all account
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task<AccountViewModel?> MapUserToAccount(ApplicationUser user)
  {
    if (user is null)
      return null;

    var account = new AccountViewModel
    {
      Name = user.DisplayName ?? string.Empty,
      Email = user.Email ?? string.Empty,
      Banned = user.Banned
    };

    if (user.LastLogin.HasValue)
      account.LastLogin = DateOnly.FromDateTime(user.LastLogin.Value);

    var roles = await userManager.GetRolesAsync(user);
    var firstRole = roles.FirstOrDefault();
    if (firstRole is not null)
      account.Role = firstRole;

    return account;
  }

  /// <summary>
  /// Sends the email to the given user that their account was approved
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private async Task SendApprovalEmailAsync(ApplicationUser user)
  {
    try
    {
      var loginLink = urlService.GetAbsoluteUrl(nameof(Account), nameof(Account.Login));
      var encodedLoginLink = HtmlEncoder.Default.Encode(loginLink);

      var subject = textService.GetText(Constants.Text.Accounts.ApprovalEmailSubject);
      var body = textService.GetText(Constants.Text.Accounts.ApprovalEmailBody);
      body = string.Format(body, encodedLoginLink);

      await emailService.SendEmailAsync(user.Email!, subject, body, EmailType.System);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send approval email for user {email}", user.Email);
    }
  }

  /// <summary>
  /// Sends the email to the given user where they can confirm
  /// their email address and set their initial password
  /// </summary>
  /// <param name="user"></param>
  /// <param name="confirmationLink"></param>
  /// <returns></returns>
  private async Task SendSetPasswordEmailAsync(ApplicationUser user, string confirmationLink)
  {
    try
    {
      var subject = textService.GetText(Constants.Text.Accounts.SetPasswordEmailSubject);
      var body = textService.GetText(Constants.Text.Accounts.SetPasswordEmailBody);
      body = string.Format(body, confirmationLink);

      await emailService.SendEmailAsync(user.Email!, subject, body, EmailType.System);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to send set password email for user {email}", user.Email);
    }
  }

  /// <summary>
  /// Maps the given user to a view model
  /// for forms acting on that account
  /// </summary>
  /// <param name="user"></param>
  /// <returns></returns>
  private static AccountActionViewModel CreateAccountActionViewModel(ApplicationUser user)
  {
    return new AccountActionViewModel
    {
      Name = user.DisplayName ?? string.Empty,
      Email = user.Email ?? string.Empty,
      Created = DateOnly.FromDateTime(user.Created)
    };
  }

  /// <summary>
  /// Returns the link for the given user where they can confirm
  /// their email address and set their initial password
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
      var url = urlService.GetAbsoluteUrl(nameof(Accounts), nameof(SetPassword), new { userId, code });
      return HtmlEncoder.Default.Encode(url);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Failed to generate confirmation link for user {email}", user.Email);
      return string.Empty;
    }
  }
}
