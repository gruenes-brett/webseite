namespace GruenesBrett.ViewModels.Accounts;

public class AccountViewModel
{
  public required string Name { get; init; }

  public required string Email { get; init; }

  public string? Role { get; set; }

  public DateOnly? LastLogin { get; set; }

  public bool Banned { get; init; }
}
