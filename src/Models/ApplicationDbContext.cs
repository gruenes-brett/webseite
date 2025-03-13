using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GruenesBrett.Models;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
  public DbSet<Audit> Audits { get; set; }

  public DbSet<Category> Categories { get; set; }

  public DbSet<PostCode> PostCodes { get; set; }

  public DbSet<Setting> Settings { get; set; }

  public DbSet<SingleEvent> SingleEvents { get; set; }

  public DbSet<Text> Texts { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasPostgresExtension("postgis");

    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<SingleEvent>()
                .HasOne(e => e.PrimaryCategory)
                .WithMany();

    modelBuilder.Entity<SingleEvent>()
                .HasMany(e => e.AdditionalCategories)
                .WithMany();
  }
}
