namespace GruenesBrett.ViewModels.Accounts;

public class AccountViewModel
{
  public required string Name { get; set; }

  public required string Email { get; set; }

  public string? Role { get; set; }

  public DateOnly? LastLogin { get; set; }

  public bool Banned { get; set; }
}
