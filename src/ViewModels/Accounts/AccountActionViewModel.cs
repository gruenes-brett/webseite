namespace GruenesBrett.ViewModels.Accounts;

public class AccountActionViewModel
{
  public required string Name { get; set; }

  public required string Email { get; set; }

  public required DateOnly Created { get; set; }
}
