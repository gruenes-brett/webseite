using GruenesBrett.Properties;

namespace GruenesBrett;

/// <summary>
/// Constant values
/// </summary>
internal static class Constants
{
  /// <summary>
  /// System related constants
  /// </summary>
  internal static class System
  {
    internal const string DataFolder = "Data";
    internal const string UploadFolder = "upload";

    internal const string ProjectUrl = "https://github.com/gruenes-brett/webseite/";
    internal const string ApiReference = "/api-reference";

    internal const string ExternalIdAlphabet = "abcdefghjkmnopqrstuvwxyz23456789";
  }

  /// <summary>
  /// ViewData related constants
  /// </summary>
  internal static class ViewData
  {
    internal const string Title = nameof(Title);
    internal const string Description = nameof(Description);
    internal const string Permalink = nameof(Permalink);
    internal const string Image = nameof(Image);

    internal const string Styles = nameof(Styles);
    internal const string Scripts = nameof(Scripts);
  }

  /// <summary>
  /// Cookie related constants
  /// </summary>
  internal static class Cookie
  {
    internal const string Application = "application";
    internal const string Antiforgery = "antiforgery";
    internal const string PostCode = "postCode";
    internal const string SearchDistance = "searchDistance";
    internal const string SelectedCategories = "selectedCategories";
    internal const string Status = "status";

    internal static readonly CookieOptions Options = new()
    {
      HttpOnly = true,
      MaxAge = TimeSpan.FromDays(30),
      SameSite = SameSiteMode.Strict
    };
  }

  /// <summary>
  /// Account related constants
  /// </summary>
  internal static class Account
  {
    internal static readonly TimeSpan DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    internal const int MaxFailedAccessAttempts = 5;
    internal const int MinimumPasswordLength = 15;
    internal const int MaximumPasswordLength = 100;
  }

  /// <summary>
  /// Roles related constants
  /// </summary>
  internal static class Roles
  {
    internal const string Administrator = nameof(Administrator);
    internal const string Editor = nameof(Editor);
    internal const string Normal = nameof(Normal);

    internal static readonly List<string> All = [Normal, Editor, Administrator];

    internal static string GetDisplayName(string role)
    {
      return role switch
      {
        Administrator => SystemTexts.Administrator,
        Editor => SystemTexts.Editor,
        Normal => SystemTexts.Normal,
        _ => string.Empty,
      };
    }
  }

  /// <summary>
  /// Event status related constants
  /// </summary>
  internal static class Status
  {
    internal const string AllStatuses = nameof(AllStatuses);
    internal const string Approved = nameof(Approved);
    internal const string WaitingForApproval = nameof(WaitingForApproval);
    internal const string Rejected = nameof(Rejected);
    internal const string Past = nameof(Past);

    internal static readonly List<string> All = [AllStatuses, Approved, WaitingForApproval, Rejected, Past];

    internal static string GetDisplayName(string status)
    {
      return status switch
      {
        AllStatuses => SystemTexts.AllStatuses,
        Approved => SystemTexts.Approved,
        WaitingForApproval => SystemTexts.WaitingForApproval,
        Rejected => SystemTexts.Rejected,
        Past => SystemTexts.Past,
        _ => string.Empty,
      };
    }
  }

  /// <summary>
  /// Pages related constants
  /// </summary>
  internal static class Pages
  {
    internal const string Explore = nameof(Explore);
    internal const string Calendar = nameof(Calendar);
    internal const string Map = nameof(Map);
    internal const string MyAccount = nameof(MyAccount);
    internal const string CreateEvent = nameof(CreateEvent);

    internal static readonly List<string> All = [Explore, Calendar, Map, MyAccount, CreateEvent];

    internal static string GetDisplayName(string page)
    {
      return page switch
      {
        Explore => SystemTexts.Explore,
        Calendar => SystemTexts.Calendar,
        Map => SystemTexts.Map,
        MyAccount => SystemTexts.MyAccount,
        CreateEvent => SystemTexts.CreateEvent,
        _ => string.Empty,
      };
    }
  }

  /// <summary>
  /// Settings related constants
  /// </summary>
  internal static class Settings
  {
    internal const string GlobalOptOut = nameof(GlobalOptOut);
    internal const string MaximumEventLength = nameof(MaximumEventLength);
    internal const string SelfRegistration = nameof(SelfRegistration);
  }

  /// <summary>
  /// Audit related constants
  /// </summary>
  internal static class Audit
  {
    internal const string Account = nameof(Account);
    internal const string Event = nameof(Event);
  }

  /// <summary>
  /// Locations related constants
  /// </summary>
  internal static class Locations
  {
    internal const int Srid = 4326; // WGS 84

    internal const int DefaultRadius = 20; // in km
    internal const int MinRadius = 1;
    internal const int MaxRadius = 10_000;

    internal const double DefaultLatitude = 51.13;
    internal const double MinLatitude = 47.27;
    internal const double MaxLatitude = 55.12;

    internal const double DefaultLongitude = 10.41;
    internal const double MinLongitude = 5.95;
    internal const double MaxLongitude = 15.06;

    internal const int DefaultSearchDistance = 3;
    internal const int MinSearchDistance = 1; // local
    internal const int MaxSearchDistance = 5; // supraregional
  }

  /// <summary>
  /// Image related constants
  /// </summary>
  internal static class Image
  {
    internal const string Jpeg = "image/jpeg";
    internal const string Png = "image/png";
    internal const int MaxSize = 10 * 1024 * 1024; // 10 MB
    internal const int MaxWidth = 960;
  }

  /// <summary>
  /// Editorial text related constants
  /// </summary>
  internal static class Text
  {
    private static readonly HashSet<string> RichText =
    [
      Account.ConfirmAccountEmailBody,
      Account.ForgotPasswordConfirmationText,
      Account.ForgotPasswordText,
      Account.NewUserEmailBody,
      Account.NoAccountYetText,
      Account.NotConfirmed,
      Account.RegistrationBenefitsText,
      Account.RegistrationConfirmationText,
      Account.ResetPasswordConfirmationText,
      Account.ResetPasswordEmailBody,
      Accounts.ApprovalEmailBody,
      Accounts.SetPasswordConfirmationText,
      Accounts.SetPasswordEmailBody,
      Content.AboutUsText,
      Content.ChangelogText,
      Content.CookieDeclarationText,
      Content.DataPrivacyPolicyText,
      Content.ImprintText,
      Content.LicensesText,
      Content.ReportIssueText,
      Content.TermsOfServiceText,
      Error.Error403,
      Error.Error404,
      Error.Generic,
      Event.ApprovalEmailBody,
      Event.ApprovedEmailBody,
      Event.ConfirmImageRights,
      Event.ConfirmPrivacyPolicy,
      Event.CreateConfirmationText,
      Event.LoginText,
      Event.RejectedEmailBody,
      Event.ReportConfirmationText,
      Event.ReportEmailBody,
      Event.RulesText,
      Events.AdjustFilters,
      Feeds.ApiText,
      Feeds.AtomText,
      Feeds.IcalText,
      Feeds.Intro,
      Homepage.Introduction
    ];

    internal static bool IsRichText(string key) => RichText.Contains(key);

    /// <summary>
    /// Keys for Account related editorial texts
    /// </summary>
    internal static class Account
    {
      internal const string Abort = "Account.Abort";
      internal const string AreaOfResponsibility = "Account.AreaOfResponsibility";
      internal const string Banned = "Account.Banned";
      internal const string ConfirmAccountEmailBody = "Account.ConfirmAccountEmailBody";
      internal const string ConfirmAccountEmailSubject = "Account.ConfirmAccountEmailSubject";
      internal const string ConfirmEmail = "Account.ConfirmEmail";
      internal const string ConfirmEmailAlreadyConfirmed = "Account.ConfirmEmailAlreadyConfirmed";
      internal const string ConfirmEmailError = "Account.ConfirmEmailError";
      internal const string ConfirmEmailSuccess = "Account.ConfirmEmailSuccess";
      internal const string ConfirmEmailTrigger = "Account.ConfirmEmailTrigger";
      internal const string ConfirmEmailTriggerText = "Account.ConfirmEmailTriggerText";
      internal const string Created = "Account.Created";
      internal const string DefaultPageExplanation = "Account.DefaultPageExplanation";
      internal const string DisplayNameExplanation = "Account.DisplayNameExplanation";
      internal const string DisplayNamePlaceholder = "Account.DisplayNamePlaceholder";
      internal const string EditError = "Account.EditError";
      internal const string EditName = "Account.EditName";
      internal const string EditPassword = "Account.EditPassword";
      internal const string EditPasswordConfirmation = "Account.EditPasswordConfirmation";
      internal const string EditPasswordConfirmationText = "Account.EditPasswordConfirmationText";
      internal const string EmailExplanation = "Account.EmailExplanation";
      internal const string EmailPlaceholder = "Account.EmailPlaceholder";
      internal const string EmailSettings = "Account.EmailSettings";
      internal const string ForgotPassword = "Account.ForgotPassword";
      internal const string ForgotPasswordConfirmation = "Account.ForgotPasswordConfirmation";
      internal const string ForgotPasswordConfirmationText = "Account.ForgotPasswordConfirmationText";
      internal const string ForgotPasswordHeading = "Account.ForgotPasswordHeading";
      internal const string ForgotPasswordText = "Account.ForgotPasswordText";
      internal const string LockedOut = "Account.LockedOut";
      internal const string Login = "Account.Login";
      internal const string LoginFailed = "Account.LoginFailed";
      internal const string Logout = "Account.Logout";
      internal const string MyAccount = "Account.MyAccount";
      internal const string NewUserEmailBody = "Account.NewUserEmailBody";
      internal const string NewUserEmailSubject = "Account.NewUserEmailSubject";
      internal const string NoAccountYetHeading = "Account.NoAccountYetHeading";
      internal const string NoAccountYetText = "Account.NoAccountYetText";
      internal const string NotConfirmed = "Account.NotConfirmed";
      internal const string OtherSettings = "Account.OtherSettings";
      internal const string PasswordChanged = "Account.PasswordChanged";
      internal const string PasswortPlaceholder = "Account.PasswortPlaceholder";
      internal const string ReceiveOptionalEmailsExplanation = "Account.ReceiveOptionalEmailsExplanation";
      internal const string ReceiveReportingEmailsExplanation = "Account.ReceiveReportingEmailsExplanation";
      internal const string Registration = "Account.Registration";
      internal const string RegistrationBenefitsHeading = "Account.RegistrationBenefitsHeading";
      internal const string RegistrationBenefitsText = "Account.RegistrationBenefitsText";
      internal const string RegistrationConfirmation = "Account.RegistrationConfirmation";
      internal const string RegistrationConfirmationText = "Account.RegistrationConfirmationText";
      internal const string RegistrationError = "Account.RegistrationError";
      internal const string ResetPassword = "Account.ResetPassword";
      internal const string ResetPasswordConfirmation = "Account.ResetPasswordConfirmation";
      internal const string ResetPasswordConfirmationText = "Account.ResetPasswordConfirmationText";
      internal const string ResetPasswordEmailBody = "Account.ResetPasswordEmailBody";
      internal const string ResetPasswordEmailSubject = "Account.ResetPasswordEmailSubject";
      internal const string ResetPasswordError = "Account.ResetPasswordError";
      internal const string Save = "Account.Save";
      internal const string Settings = "Account.Settings";
    }

    /// <summary>
    /// Keys for Accounts related editorial texts
    /// </summary>
    internal static class Accounts
    {
      internal const string Abort = "Accounts.Abort";
      internal const string AlreadyApproved = "Accounts.AlreadyApproved";
      internal const string AlreadyHasPassword = "Accounts.AlreadyHasPassword";
      internal const string ApprovalEmailBody = "Accounts.ApprovalEmailBody";
      internal const string ApprovalEmailSubject = "Accounts.ApprovalEmailSubject";
      internal const string Approve = "Accounts.Approve";
      internal const string Ban = "Accounts.Ban";
      internal const string BanAction = "Accounts.BanAction";
      internal const string BanOrUnbanYourself = "Accounts.BanOrUnbanYourself";
      internal const string Create = "Accounts.Create";
      internal const string Created = "Accounts.Created";
      internal const string CreateError = "Accounts.CreateError";
      internal const string Delete = "Accounts.Delete";
      internal const string DeleteAction = "Accounts.DeleteAction";
      internal const string DeleteYourself = "Accounts.DeleteYourself";
      internal const string EditAccounts = "Accounts.EditAccounts";
      internal const string EditResponsibility = "Accounts.EditResponsibility";
      internal const string EditRole = "Accounts.EditRole";
      internal const string EditRoleOfYourself = "Accounts.EditRoleOfYourself";
      internal const string LastLogin = "Accounts.LastLogin";
      internal const string Links = "Accounts.Links";
      internal const string Message = "Accounts.Message";
      internal const string SetPassword = "Accounts.SetPassword";
      internal const string SetPasswordConfirmation = "Accounts.SetPasswordConfirmation";
      internal const string SetPasswordConfirmationText = "Accounts.SetPasswordConfirmationText";
      internal const string SetPasswordEmailBody = "Accounts.SetPasswordEmailBody";
      internal const string SetPasswordEmailSubject = "Accounts.SetPasswordEmailSubject";
      internal const string ShowAuditLog = "Accounts.ShowAuditLog";
      internal const string Timestamp = "Accounts.Timestamp";
      internal const string Unban = "Accounts.Unban";
      internal const string UnbanAction = "Accounts.UnbanAction";
    }

    /// <summary>
    /// Keys for Content related editorial texts
    /// </summary>
    internal static class Content
    {
      internal const string AboutUs = "Content.AboutUs";
      internal const string AboutUsText = "Content.AboutUsText";
      internal const string Changelog = "Content.Changelog";
      internal const string ChangelogText = "Content.ChangelogText";
      internal const string CookieDeclaration = "Content.CookieDeclaration";
      internal const string CookieDeclarationText = "Content.CookieDeclarationText";
      internal const string DataPrivacyPolicy = "Content.DataPrivacyPolicy";
      internal const string DataPrivacyPolicyText = "Content.DataPrivacyPolicyText";
      internal const string FinancedBy = "Content.FinancedBy";
      internal const string Imprint = "Content.Imprint";
      internal const string ImprintText = "Content.ImprintText";
      internal const string Licenses = "Content.Licenses";
      internal const string LicensesText = "Content.LicensesText";
      internal const string Links = "Content.Links";
      internal const string ProjectBy = "Content.ProjectBy";
      internal const string ReportIssue = "Content.ReportIssue";
      internal const string ReportIssueText = "Content.ReportIssueText";
      internal const string TermsOfService = "Content.TermsOfService";
      internal const string TermsOfServiceText = "Content.TermsOfServiceText";
    }

    /// <summary>
    /// Keys for Error related editorial texts
    /// </summary>
    internal static class Error
    {
      internal const string Error403 = "Error.Error403";
      internal const string Error404 = "Error.Error404";
      internal const string Generic = "Error.Generic";
      internal const string Heading = "Error.Heading";
    }

    /// <summary>
    /// Keys for Event related editorial texts
    /// </summary>
    internal static class Event
    {
      internal const string Abort = "Event.Abort";
      internal const string ApprovalEmailBody = "Event.ApprovalEmailBody";
      internal const string ApprovalEmailSubject = "Event.ApprovalEmailSubject";
      internal const string Approve = "Event.Approve";
      internal const string ApprovedEmailBody = "Event.ApprovedEmailBody";
      internal const string ApprovedEmailSubject = "Event.ApprovedEmailSubject";
      internal const string CategoryExplanation = "Event.CategoryExplanation";
      internal const string ConfirmImageRights = "Event.ConfirmImageRights";
      internal const string ConfirmPrivacyPolicy = "Event.ConfirmPrivacyPolicy";
      internal const string CreateConfirmation = "Event.CreateConfirmation";
      internal const string CreateConfirmationText = "Event.CreateConfirmationText";
      internal const string Created = "Event.Created";
      internal const string CreatedBy = "Event.CreatedBy";
      internal const string CreatedByEmailExplanation = "Event.CreatedByEmailExplanation";
      internal const string CreatedByEmailPlaceholder = "Event.CreatedByEmailPlaceholder";
      internal const string CreatedByNameExplanation = "Event.CreatedByNameExplanation";
      internal const string CreatedByNamePlaceholder = "Event.CreatedByNamePlaceholder";
      internal const string CreateError = "Event.CreateError";
      internal const string DateExplanation = "Event.DateExplanation";
      internal const string DatePlaceholder = "Event.DatePlaceholder";
      internal const string Delete = "Event.Delete";
      internal const string Edit = "Event.Edit";
      internal const string EditError = "Event.EditError";
      internal const string EventDescriptionExplanation = "Event.EventDescriptionExplanation";
      internal const string EventDescriptionPlaceholder = "Event.EventDescriptionPlaceholder";
      internal const string EventImageExplanation = "Event.EventImageExplanation";
      internal const string EventLinkExplanation = "Event.EventLinkExplanation";
      internal const string EventLinkPlaceholder = "Event.EventLinkPlaceholder";
      internal const string EventNameExplanation = "Event.EventNameExplanation";
      internal const string EventNamePlaceholder = "Event.EventNamePlaceholder";
      internal const string Information = "Event.Information";
      internal const string InvalidDate = "Event.InvalidDate";
      internal const string InvalidDates = "Event.InvalidDates";
      internal const string InvalidEventLength = "Event.InvalidEventLength";
      internal const string InvalidImage = "Event.InvalidImage";
      internal const string InvalidImageRights = "Event.InvalidImageRights";
      internal const string InvalidLink = "Event.InvalidLink";
      internal const string JoinEveryDayExplanation = "Event.JoinEveryDayExplanation";
      internal const string LocationAddressExplanation = "Event.LocationAddressExplanation";
      internal const string LocationAddressPlaceholder = "Event.LocationAddressPlaceholder";
      internal const string LocationNameExplanation = "Event.LocationNameExplanation";
      internal const string LocationNamePlaceholder = "Event.LocationNamePlaceholder";
      internal const string LoggedIn = "Event.LoggedIn";
      internal const string LoginHeading = "Event.LoginHeading";
      internal const string LoginText = "Event.LoginText";
      internal const string MoreInformation = "Event.MoreInformation";
      internal const string Name = "Event.Name";
      internal const string NotApprovedYet = "Event.NotApprovedYet";
      internal const string NotLoggedIn = "Event.NotLoggedIn";
      internal const string OrganizerNameExplanation = "Event.OrganizerNameExplanation";
      internal const string OrganizerNamePlaceholder = "Event.OrganizerNamePlaceholder";
      internal const string Reject = "Event.Reject";
      internal const string RejectedEmailBody = "Event.RejectedEmailBody";
      internal const string RejectedEmailSubject = "Event.RejectedEmailSubject";
      internal const string Report = "Event.Report";
      internal const string ReportConfirmation = "Event.ReportConfirmation";
      internal const string ReportConfirmationText = "Event.ReportConfirmationText";
      internal const string ReportDoesNotFitSite = "Event.ReportDoesNotFitSite";
      internal const string ReportEmailBody = "Event.ReportEmailBody";
      internal const string ReportEmailSubject = "Event.ReportEmailSubject";
      internal const string ReportIsCanceled = "Event.ReportIsCanceled";
      internal const string ReportIsIllegal = "Event.ReportIsIllegal";
      internal const string ReportIsNotPublic = "Event.ReportIsNotPublic";
      internal const string ReportIsOther = "Event.ReportIsOther";
      internal const string ReportIsSpam = "Event.ReportIsSpam";
      internal const string RulesHeading = "Event.RulesHeading";
      internal const string RulesText = "Event.RulesText";
      internal const string Share = "Event.Share";
      internal const string TimeExplanation = "Event.TimeExplanation";
      internal const string TimePlaceholder = "Event.TimePlaceholder";
      internal const string WasRejected = "Event.WasRejected";
    }

    /// <summary>
    /// Keys for Events related editorial texts
    /// </summary>
    internal static class Events
    {
      internal const string Actions = "Events.Actions";
      internal const string AdjustFilters = "Events.AdjustFilters";
      internal const string ApplyFilters = "Events.ApplyFilters";
      internal const string Approve = "Events.Approve";
      internal const string Approved = "Events.Approved";
      internal const string Categories = "Events.Categories";
      internal const string ChangeLocation = "Events.ChangeLocation";
      internal const string Created = "Events.Created";
      internal const string CreatedBy = "Events.CreatedBy";
      internal const string Delete = "Events.Delete";
      internal const string Edit = "Events.Edit";
      internal const string EditAction = "Events.EditAction";
      internal const string Filter = "Events.Filter";
      internal const string Links = "Events.Links";
      internal const string Local = "Events.Local";
      internal const string Location = "Events.Location";
      internal const string LoggedIn = "Events.LoggedIn";
      internal const string Message = "Events.Message";
      internal const string MoreInformation = "Events.MoreInformation";
      internal const string Name = "Events.Name";
      internal const string NoResults = "Events.NoResults";
      internal const string NotLoggedIn = "Events.NotLoggedIn";
      internal const string Rejected = "Events.Rejected";
      internal const string SearchDistance = "Events.SearchDistance";
      internal const string ShowAuditLog = "Events.ShowAuditLog";
      internal const string Supraregional = "Events.Supraregional";
      internal const string Timestamp = "Events.Timestamp";
      internal const string WaitingForApproval = "Events.WaitingForApproval";
    }

    /// <summary>
    /// Keys for Feeds related editorial texts
    /// </summary>
    internal static class Feeds
    {
      internal const string ApiLink = "Feeds.ApiLink";
      internal const string ApiText = "Feeds.ApiText";
      internal const string ApiTitle = "Feeds.ApiTitle";
      internal const string AtomLink = "Feeds.AtomLink";
      internal const string AtomText = "Feeds.AtomText";
      internal const string AtomTitle = "Feeds.AtomTitle";
      internal const string IcalLink = "Feeds.IcalLink";
      internal const string IcalText = "Feeds.IcalText";
      internal const string IcalTitle = "Feeds.IcalTitle";
      internal const string Index = "Feeds.Index";
      internal const string Intro = "Feeds.Intro";
    }

    /// <summary>
    /// Keys for Homepage related editorial texts
    /// </summary>
    internal static class Homepage
    {
      internal const string Index = "Homepage.Index";
      internal const string Introduction = "Homepage.Introduction";
      internal const string ProjectBy = "Homepage.ProjectBy";
      internal const string SearchCityOrPostCode = "Homepage.SearchCityOrPostCode";
      internal const string WhereToSearch = "Homepage.WhereToSearch";
    }

    /// <summary>
    /// Keys for Settings related editorial texts
    /// </summary>
    internal static class Settings
    {
      internal const string Abort = "Settings.Abort";
      internal const string AccountSettings = "Settings.AccountSettings";
      internal const string Action = "Settings.Action";
      internal const string AddCategory = "Settings.AddCategory";
      internal const string Delete = "Settings.Delete";
      internal const string DeleteCategory = "Settings.DeleteCategory";
      internal const string Edit = "Settings.Edit";
      internal const string EditCategories = "Settings.EditCategories";
      internal const string EditCategory = "Settings.EditCategory";
      internal const string EditSettings = "Settings.EditSettings";
      internal const string EditText = "Settings.EditText";
      internal const string EditTexts = "Settings.EditTexts";
      internal const string EmailSettings = "Settings.EmailSettings";
      internal const string EventSettings = "Settings.EventSettings";
      internal const string GlobalOptOutExplanation = "Settings.GlobalOptOutExplanation";
      internal const string Key = "Settings.Key";
      internal const string Links = "Settings.Links";
      internal const string MaximumEventLengthExplanation = "Settings.MaximumEventLengthExplanation";
      internal const string Save = "Settings.Save";
      internal const string SelfRegistrationExplanation = "Settings.SelfRegistrationExplanation";
      internal const string Value = "Settings.Value";
    }
  }
}
