using System.Collections.Concurrent;
using GruenesBrett.Interfaces;
using GruenesBrett.Models;

namespace GruenesBrett.Services;

public sealed class CategoryService : ICategoryService
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ConcurrentDictionary<Guid, Category> _cache = new();

  /// <summary>
  /// Constructor for initializing the cache
  /// </summary>
  /// <param name="scopeFactory"></param>
  public CategoryService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
    RefreshAllCategories();
  }

  /// <inheritdoc />
  public HashSet<Category> GetAllCategoriesAsSet()
  {
    return [.. _cache.Values];
  }

  /// <inheritdoc />
  public List<Category> GetAllCategoriesSorted()
  {
    return [.. _cache.Values.OrderBy(c => c.Name)];
  }

  /// <inheritdoc />
  public HashSet<Category> GetCategories(List<Guid> ids)
  {
    if (ids.Count == 0)
      return [];

    var categories = new HashSet<Category>(ids.Count);
    foreach (var id in ids)
    {
      if (_cache.TryGetValue(id, out var category))
        categories.Add(category);
    }

    return categories;
  }

  /// <inheritdoc />
  public void RefreshAllCategories()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    _cache.Clear();

    foreach (var category in context.Categories)
      _cache.TryAdd(category.Id, category);
  }
}
