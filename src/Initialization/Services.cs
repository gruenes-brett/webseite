using GruenesBrett.Email;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;
using GruenesBrett.Services;
using Linkernizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Geometries;
using Serilog;
using Sqids;

namespace GruenesBrett.Initialization;

/// <summary>
/// Handles configuring services during initialization
/// </summary>
internal static class Services
{
  /// <summary>
  /// Configures all services on the given builder
  /// </summary>
  /// <param name="builder"></param>
  internal static void Configure(WebApplicationBuilder builder)
  {
    // adds default routing and make sure that most URLs (excluding some identity related ones)
    // are generated with only lowercase letters
    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    // configures the exception page if migrations were not applied during initialization
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    var mainDatabaseConnectionString = builder.Configuration.GetConnectionString("MainDatabase");

    // configures the main Entity Framework database context to use spatial data and seeding
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
      options.UseNpgsql(mainDatabaseConnectionString, b => b.UseNetTopologySuite(geographyAsDefault: true))
      .UseSeeding((_, _) => { })
      .UseAsyncSeeding(async (context, _, _) =>
      {
        Console.WriteLine("Starting text seeding");
        await Seeder.SeedTextsAsync(context);
        Console.WriteLine("Finished text seeding");
        Console.WriteLine("Starting category seeding");
        await Seeder.SeedCategoriesAsync(context);
        Console.WriteLine("Finished category seeding");
        Console.WriteLine("Starting postcode seeding");
        await Seeder.SeedPostCodesAsync(context);
        Console.WriteLine("Finished postcode seeding");
      });
    });

    // configures Entity Framework Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
      options.Lockout.DefaultLockoutTimeSpan = Constants.Account.DefaultLockoutTimeSpan;
      options.Lockout.MaxFailedAccessAttempts = Constants.Account.MaxFailedAccessAttempts;
      options.Password.RequiredLength = Constants.Account.MinimumPasswordLength;
      options.Password.RequireDigit = false;
      options.Password.RequireLowercase = false;
      options.Password.RequireUppercase = false;
      options.Password.RequireNonAlphanumeric = false;
      options.SignIn.RequireConfirmedAccount = true;
    })
      .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
      options.TokenLifespan = TimeSpan.FromDays(14)
    );

    // configures all MVC controllers
    builder.Services.AddControllersWithViews();

    // configures the REST API documentation
    builder.Services.AddOpenApi("v1", options =>
    {
      options.AddDocumentTransformer((document, _, _) =>
      {
        document.Info.Title = "Grünes Brett API";
        document.Info.Description = "Provides access to all public data about all events shown on the Grünes Brett website.";
        document.Info.Contact = new OpenApiContact()
        {
          Name = "Source code",
          Url = new Uri(Constants.System.ProjectUrl)
        };

        return Task.CompletedTask;
      });

      options.AddSchemaTransformer((schema, context, _) =>
      {
        if (context.JsonTypeInfo.Type == typeof(Point))
        {
          schema.Format = "";
          schema.Type = "";
        }
        return Task.CompletedTask;
      });
    });

    // configure some identity related URLs to be lowercase
    // and rename default cookies to better represent their functionality
    builder.Services.ConfigureApplicationCookie(config =>
    {
      config.LoginPath = "/account/anmelden";
      config.LogoutPath = "/account/abmelden";
      config.AccessDeniedPath = "/fehler/403";
      config.Cookie.Name = Constants.Cookie.Application;
      config.SlidingExpiration = true;
    });
    builder.Services.AddAntiforgery(options => options.Cookie.Name = Constants.Cookie.Antiforgery);

    // read the email options from the configuration
    builder.Services.Configure<MailKitEmailSenderOptions>(options =>
    {
      options.Address = builder.Configuration["Email:Address"] ?? string.Empty;
      options.Port = int.TryParse(builder.Configuration["Email:Port"], out var port) ? port : 0;
      options.Username = builder.Configuration["Email:Username"] ?? string.Empty;
      options.Password = builder.Configuration["Email:Password"] ?? string.Empty;
      options.SenderEmail = builder.Configuration["Email:SenderEmail"] ?? string.Empty;
      options.SenderName = builder.Configuration["Email:SenderName"] ?? string.Empty;
    });

    // register singleton services for use with dependency injection
    builder.Services.AddSingleton<ICategoryService, CategoryService>();
    builder.Services.AddSingleton<ILinkernizer>(new Linkernizer.Linkernizer(options =>
      options.OpenExternalLinksInNewTab = true
    ));
    builder.Services.AddSingleton<IPostCodeService, PostCodeService>();
    builder.Services.AddSingleton<ITextService, TextService>();
    builder.Services.AddSingleton(new SqidsEncoder<long>(new()
    {
      Alphabet = Constants.System.ExternalIdAlphabet
    }));

    // register transient services for use with dependency injection
    builder.Services.AddTransient<IAuditService, AuditService>();
    builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddTransient<IEventService, EventService>();
    builder.Services.AddTransient<IFilterService, FilterService>();
    builder.Services.AddTransient<ILocationService, LocationService>();
    builder.Services.AddTransient<ISettingsService, SettingsService>();
    builder.Services.AddTransient<IUrlService, UrlService>();
    builder.Services.AddTransient<IUserService, UserService>();

    // configure logging
    builder.Host.UseSerilog((context, configuration) =>
      configuration.ReadFrom.Configuration(context.Configuration)
    );
  }
}
