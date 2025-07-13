namespace GruenesBrett.ViewModels.Accounts;

public class AccountActionViewModel
{
  public required string Name { get; init; }

  public required string Email { get; init; }

  public required DateOnly Created { get; init; }
}
