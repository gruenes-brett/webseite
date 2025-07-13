using System.Collections.Concurrent;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;

namespace GruenesBrett.Services;

public class PostCodeService : IPostCodeService
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ConcurrentDictionary<string, PostCode> _cache = new();

  /// <summary>
  /// Constructor to initialize the cache
  /// </summary>
  /// <param name="scopeFactory"></param>
  public PostCodeService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
    InitializeCache();
  }

  /// <inheritdoc />
  public PostCode? GetPostCode(string key)
  {
    if (key.Length < 5)
      return null;

    var postCodeId = key[..5];

    _cache.TryGetValue(postCodeId, out var postCode);
    return postCode;
  }

  /// <inheritdoc />
  public IEnumerable<PostCode> GetAllPostCodes()
  {
    return _cache.Values;
  }

  /// <inheritdoc />
  public IEnumerable<string> GetMatchingPostCodeNames(string query)
  {
    var matchingPostCodes = _cache.Values.Where(postCode => postCode.FullName.Contains(query, StringComparison.CurrentCultureIgnoreCase));
    var matchingPostCodeNames = matchingPostCodes.Select(postCode => postCode.FullName);
    return matchingPostCodeNames.Order();
  }

  /// <summary>
  /// Reads all post codes from the database into the cache
  /// </summary>
  private void InitializeCache()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    foreach (var postCode in context.PostCodes)
      _cache.AddOrUpdate(postCode.Id, postCode, (_, _) => postCode);
  }
}
